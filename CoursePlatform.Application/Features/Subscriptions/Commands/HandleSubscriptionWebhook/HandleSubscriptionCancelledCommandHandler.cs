using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.HandleSubscriptionWebhook;

public class HandleSubscriptionCancelledCommandHandler
    : IRequestHandler<HandleSubscriptionCancelledCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notifications;

    public HandleSubscriptionCancelledCommandHandler(
        IUnitOfWork uow,
        INotificationService notifications)
    {
        _uow = uow;
        _notifications = notifications;
    }

    public async Task<Unit> Handle(
        HandleSubscriptionCancelledCommand request, CancellationToken ct)
    {
        var spec = new SubscriptionByStripeIdSpec(
            request.StripeSubscriptionId);
        var subscription = await _uow.Repository<UserSubscription>()
                                     .GetEntityWithSpecAsync(spec, ct);

        if (subscription is null) return Unit.Value;

        subscription.Status = SubscriptionStatus.Expired;
        subscription.AutoRenew = false;

        _uow.Repository<UserSubscription>().Update(subscription);
        await _uow.CompleteAsync(ct);

        await _notifications.SendAsync(
            userId: subscription.UserId,
            title: "Subscription Expired",
            message: "Your subscription has expired. Renew to regain access.",
            type: NotificationType.SystemMessage,
            actionUrl: "/subscription",
            ct: ct);

        return Unit.Value;
    }
}