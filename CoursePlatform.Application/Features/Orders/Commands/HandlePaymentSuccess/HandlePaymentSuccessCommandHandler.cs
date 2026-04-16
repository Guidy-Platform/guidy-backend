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

    public HandlePaymentSuccessCommandHandler(
        IUnitOfWork uow,
        IMessagePublisher publisher)
    {
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(
        HandlePaymentSuccessCommand request, CancellationToken ct)
    {
        // use the PaymentIntentId to find the corresponding Order
        var spec = new OrderByPaymentIntentSpec(request.PaymentIntentId);
        var order = await _uow.Repository<Order>()
                              .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException(
                $"Order with PaymentIntent '{request.PaymentIntentId}' not found.");

        if (order.Status == OrderStatus.Completed)
            return Unit.Value;

        // 2. Complete Order
        order.Status = OrderStatus.Completed;
        order.PaidAt = DateTime.UtcNow;
        _uow.Repository<Order>().Update(order);

        // 3. Create Enrollments for each course in the order
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

        // 4. Publish event على RabbitMQ للـ notification
        await _publisher.PublishAsync(new OrderCompletedEvent
        {
            OrderId = order.Id,
            StudentId = order.StudentId,
            FinalPrice = order.FinalPrice,
            CourseIds = order.OrderItems.Select(i => i.CourseId).ToList(),
            CourseTitles = order.OrderItems.Select(i => i.CourseTitle).ToList()
        }, "order.completed", ct);

        return Unit.Value;
    }
}