using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Orders.Events;
using CoursePlatform.Application.Features.Orders.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.HandlePaymentSuccess;

public class HandlePaymentSuccessCommandHandler
    : IRequestHandler<HandlePaymentSuccessCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IMessagePublisher _publisher;
    private readonly INotificationService _notificationService;  
    private readonly IUserRepository _userRepo;             

    public HandlePaymentSuccessCommandHandler(
        IUnitOfWork uow,
        IMessagePublisher publisher,
        INotificationService notificationService,
        IUserRepository userRepo)
    {
        _uow = uow;
        _publisher = publisher;
        _notificationService = notificationService;
        _userRepo = userRepo;
    }

    public async Task<Unit> Handle(
        HandlePaymentSuccessCommand request, CancellationToken ct)
    {
        var spec = new OrderByPaymentIntentSpec(request.PaymentIntentId);
        var order = await _uow.Repository<Order>()
                              .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException(
                $"Order with PaymentIntent '{request.PaymentIntentId}' not found.");

        if (order.Status == OrderStatus.Completed)
            return Unit.Value;

        order.Status = OrderStatus.Completed;
        order.PaidAt = DateTime.UtcNow;
        _uow.Repository<Order>().Update(order);

        foreach (var item in order.OrderItems)
        {
            var enrollment = new Enrollment
            {
                StudentId = order.StudentId,
                CourseId = item.CourseId,
                OrderId = order.Id,
                EnrolledAt = DateTime.UtcNow
            };
            await _uow.Repository<Enrollment>().AddAsync(enrollment, ct);
        }

        await _uow.CompleteAsync(ct);

        // جيب الـ student للـ email
        var student = await _userRepo.GetByIdAsync(order.StudentId, ct);

        var courseList = string.Join(", ",
            order.OrderItems.Select(i => i.CourseTitle));

        // In-app notification
        await _notificationService.SendAsync(
            userId: order.StudentId,
            title: "Enrollment Confirmed!",
            message: $"You are now enrolled in: {courseList}",
            type: NotificationType.OrderCompleted,
            actionUrl: "/my-courses",
            sendEmail: student is not null,
            emailAddress: student?.Email,
            ct: ct);

        // Instructor notification لكل course
        foreach (var item in order.OrderItems)
        {
            var course = await _uow.Repository<Course>()
                                   .GetByIdAsync(item.CourseId, ct);
            if (course is null) continue;

            await _notificationService.SendAsync(
                userId: course.InstructorId,
                title: "New Enrollment!",
                message: $"A new student enrolled in '{item.CourseTitle}'.",
                type: NotificationType.NewEnrollment,
                actionUrl: $"/instructor/courses/{item.CourseId}",
                ct: ct);
        }

        // Publish event للـ RabbitMQ (للـ email confirmation)
        await _publisher.PublishAsync(new OrderCompletedEvent
        {
            OrderId = order.Id,
            StudentId = order.StudentId,
            StudentEmail = student?.Email ?? string.Empty,
            StudentName = student?.FullName ?? string.Empty,
            FinalPrice = order.FinalPrice,
            CourseIds = order.OrderItems.Select(i => i.CourseId).ToList(),
            CourseTitles = order.OrderItems.Select(i => i.CourseTitle).ToList()
        }, "order.completed", ct);

        return Unit.Value;
    }
}