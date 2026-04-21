using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.HandleSubscriptionWebhook;

public record HandleSubscriptionRenewedCommand(
    string StripeSubscriptionId,
    DateTime NewPeriodEnd
) : IRequest<Unit>;