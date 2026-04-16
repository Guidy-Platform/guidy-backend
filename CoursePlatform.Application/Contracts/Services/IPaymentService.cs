namespace CoursePlatform.Application.Contracts.Services;

public record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    long AmountInCents
);

public interface IPaymentService
{
    // create a payment intent for the given amount, return the payment intent id and client secret
    Task<PaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        int orderId,
        CancellationToken ct = default);

    Task UpdatePaymentIntentAsync(                  
     string paymentIntentId, decimal newAmount,
     CancellationToken ct = default);

    bool ValidateWebhookSignature(
        string payload,
        string signature,
        out string eventType,
        out string paymentIntentId);
}