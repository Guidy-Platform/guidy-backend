using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Subscriptions.DTOs;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;

public class SubscribeCommandHandler
    : IRequestHandler<SubscribeCommand, UserSubscriptionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepo;
    private readonly IStripeSubscriptionService _stripe;
    private readonly INotificationService _notifications;

    public SubscribeCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IUserRepository userRepo,
        IStripeSubscriptionService stripe,
        INotificationService notifications)
    {
        _uow = uow;
        _currentUser = currentUser;
        _userRepo = userRepo;
        _stripe = stripe;
        _notifications = notifications;
    }

    public async Task<UserSubscriptionDto> Handle(
        SubscribeCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // 1. تحقق مفيش active subscription
        var existingSpec = new ActiveSubscriptionByUserSpec(userId);
        var hasActive = await _uow.Repository<UserSubscription>()
                                     .AnyAsync(existingSpec, ct);
        if (hasActive)
            throw new ConflictException(
                "You already have an active subscription. " +
                "Cancel it first before subscribing to a new plan.");

        // 2. جيب الـ Plan
        var plan = await _uow.Repository<SubscriptionPlan>()
                             .GetByIdAsync(request.PlanId, ct)
            ?? throw new NotFoundException("SubscriptionPlan", request.PlanId);

        if (!plan.IsActive)
            throw new BadRequestException("This subscription plan is not available.");

        if (string.IsNullOrEmpty(plan.StripePriceId))
            throw new BadRequestException(
                "This plan is not configured for payment yet.");

        // 3. جيب الـ User
        var user = await _userRepo.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);

        // 4. Stripe Subscription
        var stripeResult = await _stripe.CreateSubscriptionAsync(
            user.Email!, plan.StripePriceId, ct);

        // 5. Save في الـ DB (Pending حتى الـ Webhook يأكد)
        var subscription = new UserSubscription
        {
            UserId = userId,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = stripeResult.CurrentPeriodEnd,
            AutoRenew = true,
            StripeSubscriptionId = stripeResult.SubscriptionId,
            StripeCustomerId = stripeResult.CustomerId
        };

        await _uow.Repository<UserSubscription>().AddAsync(subscription, ct);
        await _uow.CompleteAsync(ct);

        // 6. Notification
        await _notifications.SendAsync(
            userId: userId,
            title: "Subscription Activated!",
            message: $"You now have full access to all courses with {plan.Name}.",
            type: NotificationType.SystemMessage,
            actionUrl: "/courses",
            ct: ct);

        return MapToDto(subscription, plan, stripeResult.ClientSecret);
    }

    internal static UserSubscriptionDto MapToDto(
        UserSubscription subscription,
        SubscriptionPlan plan,
        string? clientSecret = null) => new()
        {
            Id = subscription.Id,
            PlanName = plan.Name,
            BillingInterval = plan.BillingInterval.ToString(),
            Price = plan.Price,
            Status = subscription.Status.ToString(),
            IsActive = subscription.IsActive,
            AutoRenew = subscription.AutoRenew,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            CancelledAt = subscription.CancelledAt,
            DaysRemaining = Math.Max(
            0, (int)(subscription.EndDate - DateTime.UtcNow).TotalDays),
            ClientSecret = clientSecret
        };
}