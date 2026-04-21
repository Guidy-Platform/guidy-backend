namespace CoursePlatform.Application.Contracts.Services;

public record CreateSubscriptionResult(
    string SubscriptionId,
    string CustomerId,
    string ClientSecret,   // for Frontend payment confirmation
    DateTime CurrentPeriodEnd
);

public interface IStripeSubscriptionService
{
    Task<CreateSubscriptionResult> CreateSubscriptionAsync(
        string customerEmail,
        string stripePriceId,
        CancellationToken ct = default);

    Task CancelSubscriptionAsync(
        string stripeSubscriptionId,
        CancellationToken ct = default);

    bool ValidateWebhookSignature(
        string payload,
        string signature,
        out string eventType,
        out string subscriptionId,
        out DateTime? periodEnd);
}