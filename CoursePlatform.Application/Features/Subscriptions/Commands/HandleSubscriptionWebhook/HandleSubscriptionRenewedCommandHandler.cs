using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.HandleSubscriptionWebhook;

public class HandleSubscriptionRenewedCommandHandler
    : IRequestHandler<HandleSubscriptionRenewedCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<HandleSubscriptionRenewedCommandHandler> _logger;

    public HandleSubscriptionRenewedCommandHandler(
        IUnitOfWork uow,
        ILogger<HandleSubscriptionRenewedCommandHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        HandleSubscriptionRenewedCommand request, CancellationToken ct)
    {
        var spec = new SubscriptionByStripeIdSpec(
            request.StripeSubscriptionId);
        var subscription = await _uow.Repository<UserSubscription>()
                                     .GetEntityWithSpecAsync(spec, ct);

        if (subscription is null)
        {
            _logger.LogWarning(
                "Subscription not found for Stripe ID: {Id}",
                request.StripeSubscriptionId);
            return Unit.Value;
        }

        // Idempotent
        if (subscription.EndDate == request.NewPeriodEnd)
            return Unit.Value;

        subscription.Status = SubscriptionStatus.Active;
        subscription.EndDate = request.NewPeriodEnd;
        subscription.AutoRenew = true;

        _uow.Repository<UserSubscription>().Update(subscription);
        await _uow.CompleteAsync(ct);

        _logger.LogInformation(
            "Subscription renewed: {Id} until {End}",
            subscription.Id, request.NewPeriodEnd);

        return Unit.Value;
    }
}