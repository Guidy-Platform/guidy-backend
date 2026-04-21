// Application/Features/Subscriptions/Commands/CancelSubscription/CancelSubscriptionCommandHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;
using CoursePlatform.Application.Features.Subscriptions.DTOs;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, UserSubscriptionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IStripeSubscriptionService _stripe;
    private readonly INotificationService _notifications;

    public CancelSubscriptionCommandHandler(
        IUnitOfWork _uow,
        ICurrentUserService currentUser,
        IStripeSubscriptionService stripe,
        INotificationService notifications)
    {
        this._uow = _uow;
        _currentUser = currentUser;
        _stripe = stripe;
        _notifications = notifications;
    }

    public async Task<UserSubscriptionDto> Handle(
        CancelSubscriptionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new ActiveSubscriptionByUserSpec(userId);
        var subscription = await _uow.Repository<UserSubscription>()
                                     .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException(
                "No active subscription found.");

        // Cancel في Stripe (at period end)
        if (!string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            await _stripe.CancelSubscriptionAsync(
                subscription.StripeSubscriptionId, ct);
        }

        // حدث الـ DB
        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.AutoRenew = false;
        subscription.CancelledAt = DateTime.UtcNow;

        _uow.Repository<UserSubscription>().Update(subscription);
        await _uow.CompleteAsync(ct);

        await _notifications.SendAsync(
            userId: userId,
            title: "Subscription Cancelled",
            message: $"Your subscription has been cancelled. " +
                       $"You still have access until {subscription.EndDate:MMM dd, yyyy}.",
            type: NotificationType.SystemMessage,
            actionUrl: "/subscription",
            ct: ct);

        return SubscribeCommandHandler.MapToDto(subscription, subscription.Plan);
    }
}