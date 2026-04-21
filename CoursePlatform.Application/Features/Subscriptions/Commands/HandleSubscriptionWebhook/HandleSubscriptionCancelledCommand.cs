using MediatR;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.HandleSubscriptionWebhook;

public record HandleSubscriptionCancelledCommand(
    string StripeSubscriptionId
) : IRequest<Unit>;