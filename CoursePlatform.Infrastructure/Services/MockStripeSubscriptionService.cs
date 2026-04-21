using CoursePlatform.Application.Contracts.Services;

namespace CoursePlatform.Infrastructure.Services;

public class MockStripeSubscriptionService : IStripeSubscriptionService
{
    public Task<CreateSubscriptionResult> CreateSubscriptionAsync(
        string customerEmail, string stripePriceId,
        CancellationToken ct = default)
    {
        Console.WriteLine($"[MOCK] Creating subscription for {customerEmail}");

        var fakeSubId = $"sub_test_{Guid.NewGuid():N}"[..20];
        var fakeCustId = $"cus_test_{Guid.NewGuid():N}"[..20];
        var fakeSecret = $"pi_test_{Guid.NewGuid():N}_secret";
        var periodEnd = DateTime.UtcNow.AddMonths(1);

        return Task.FromResult(new CreateSubscriptionResult(
            fakeSubId, fakeCustId, fakeSecret, periodEnd));
    
    }

    public Task CancelSubscriptionAsync(
        string stripeSubscriptionId, CancellationToken ct = default)
        => Task.CompletedTask;

    public bool ValidateWebhookSignature(
        string payload, string signature,
        out string eventType, out string subscriptionId,
        out DateTime? periodEnd)
    {
        eventType = "customer.subscription.updated";
        subscriptionId = string.Empty;
        periodEnd = DateTime.UtcNow.AddMonths(1);
        return true;
    }
}