using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace CoursePlatform.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly string _webhookSecret;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IConfiguration config,
        ILogger<StripePaymentService> logger)
    {
        _logger = logger;
        _webhookSecret = config["Stripe:WebhookSecret"]!;

        // Set Stripe API Key globally
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"]!;
    }

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        int orderId,
        CancellationToken ct = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),  // Stripe uses cents
            Currency = currency.ToLower(),
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = orderId.ToString()
            },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options, cancellationToken: ct);

        _logger.LogInformation(
            "PaymentIntent created: {Id} for Order: {OrderId}",
            intent.Id, orderId);

        return new PaymentIntentResult(
            intent.Id,
            intent.ClientSecret,
            intent.Amount);
    }

    public async Task UpdatePaymentIntentAsync(
        string paymentIntentId, decimal newAmount,
        CancellationToken ct = default)
    {
        var options = new PaymentIntentUpdateOptions
        {
            Amount = (long)(newAmount * 100)
        };

        var service = new PaymentIntentService();
        await service.UpdateAsync(paymentIntentId, options,
            cancellationToken: ct);
    }



    public bool ValidateWebhookSignature(
        string payload,
        string signature,
        out string eventType,
        out string paymentIntentId)
    {
        eventType = string.Empty;
        paymentIntentId = string.Empty;

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                _webhookSecret,
                throwOnApiVersionMismatch: false);  // ← مهم

            eventType = stripeEvent.Type;

            // استخرج الـ PaymentIntentId من الـ event
            if (stripeEvent.Data?.Object is Stripe.PaymentIntent intent)
            {
                paymentIntentId = intent.Id;
            }

            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(
                "Stripe webhook validation failed: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating Stripe webhook.");
            return false;
        }
    }

}