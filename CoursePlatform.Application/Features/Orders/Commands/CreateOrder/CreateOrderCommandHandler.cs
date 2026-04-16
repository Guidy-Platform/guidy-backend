using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Orders.DTOs;
using CoursePlatform.Application.Features.Orders.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _payment;

    public CreateOrderCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IPaymentService payment)
    {
        _uow = uow;
        _currentUser = currentUser;
        _payment = payment;
    }

    public async Task<CreateOrderResultDto> Handle(
        CreateOrderCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // check if courses exist and are published
        var courses = new List<Course>();
        foreach (var courseId in request.CourseIds)
        {
            var course = await _uow.Repository<Course>()
                                   .GetByIdAsync(courseId, ct)
                ?? throw new NotFoundException("Course", courseId);

            if (course.Status != CourseStatus.Published)
                throw new BadRequestException(
                    $"Course '{course.Title}' is not available for purchase.");

            courses.Add(course);
        }

        // check if student is already enrolled in any of the courses
        foreach (var course in courses)
        {
            var alreadyEnrolled = await _uow.Repository<Enrollment>()
                .AnyAsync(new AlreadyEnrolledSpec(studentId, course.Id), ct);

            if (alreadyEnrolled)
                throw new ConflictException(
                    $"You are already enrolled in '{course.Title}'.");
        }

        // Calculate total price considering discounts (if any)
        var totalPrice = courses.Sum(c =>
            c.DiscountPrice.HasValue && c.DiscountPrice > 0
                ? c.DiscountPrice.Value
                : c.Price);

        // Create order
        var order = new Order
        {
            StudentId = studentId,
            TotalPrice = totalPrice,
            DiscountAmount = 0,
            FinalPrice = totalPrice,
            Status = OrderStatus.Pending,
        };

        foreach (var course in courses)
        {
            order.OrderItems.Add(new OrderItem
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                Price = course.DiscountPrice ?? course.Price
            });
        }

        await _uow.Repository<Order>().AddAsync(order, ct);
        await _uow.CompleteAsync(ct);

        // free courses should be enrolled immediately without going through payment
        if (order.FinalPrice == 0)
        {
            await EnrollStudentAsync(order, courses, studentId, ct);
            return new CreateOrderResultDto
            {
                OrderId = order.Id,
                FinalPrice = 0,
                IsFree = true,
                ClientSecret = string.Empty
            };
        }

        // create payment intent in Stripe
        var paymentResult = await _payment.CreatePaymentIntentAsync(
            order.FinalPrice, "usd", order.Id, ct);

        order.PaymentIntentId = paymentResult.PaymentIntentId;
        _uow.Repository<Order>().Update(order);
        await _uow.CompleteAsync(ct);

        return new CreateOrderResultDto
        {
            OrderId = order.Id,
            FinalPrice = order.FinalPrice,
            ClientSecret = paymentResult.ClientSecret,
            IsFree = false
        };
    }

    private async Task EnrollStudentAsync(
        Order order, List<Course> courses,
        Guid studentId, CancellationToken ct)
    {
        foreach (var course in courses)
        {
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = course.Id,
                OrderId = order.Id,
                EnrolledAt = DateTime.UtcNow
            };
            await _uow.Repository<Enrollment>().AddAsync(enrollment, ct);
        }

        order.Status = OrderStatus.Completed;
        order.PaidAt = DateTime.UtcNow;
        _uow.Repository<Order>().Update(order);

        await _uow.CompleteAsync(ct);
    }
}