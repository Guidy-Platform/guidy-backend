// Infrastructure/Services/StripeSubscriptionService.cs
using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace CoursePlatform.Infrastructure.Services;

public class StripeSubscriptionService : IStripeSubscriptionService
{
    private readonly string                             _webhookSecret;
    private readonly ILogger<StripeSubscriptionService> _logger;

    public StripeSubscriptionService(
        IConfiguration                         config,
        ILogger<StripeSubscriptionService>     logger)
    {
        _logger        = logger;
        _webhookSecret = config["Stripe:SubscriptionWebhookSecret"]
                         ?? config["Stripe:WebhookSecret"]!;
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"]!;
    }

    // Infrastructure/Services/StripeSubscriptionService.cs
    public async Task<CreateSubscriptionResult> CreateSubscriptionAsync(
        string customerEmail,
        string stripePriceId,
        CancellationToken ct = default)
    {
        // 1. Customer
        var customerService = new CustomerService();
        var existing = await customerService.ListAsync(
            new CustomerListOptions { Email = customerEmail, Limit = 1 },
            cancellationToken: ct);

        var customer = existing.Data.FirstOrDefault()
            ?? await customerService.CreateAsync(
                new CustomerCreateOptions { Email = customerEmail },
                cancellationToken: ct);

        // 2. Subscription
        var subService = new SubscriptionService();
        var sub = await subService.CreateAsync(
            new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = [new SubscriptionItemOptions { Price = stripePriceId }],
                PaymentBehavior = "default_incomplete",
                PaymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    SaveDefaultPaymentMethod = "on_subscription"
                },
                Expand = ["latest_invoice"]
            },
            cancellationToken: ct);

        // 3. Client Secret — v51 way
        // InvoiceConfirmationSecret object عنده ClientSecret property
        var clientSecret = string.Empty;

        if (sub.LatestInvoice?.ConfirmationSecret is not null)
        {
            // ConfirmationSecret هو object من نوع InvoiceConfirmationSecret
            // عنده ClientSecret property
            clientSecret = sub.LatestInvoice.ConfirmationSecret.ClientSecret
                           ?? string.Empty;
        }

        // لو ConfirmationSecret مش موجود — جيب الـ PaymentIntent بشكل منفصل
        if (string.IsNullOrEmpty(clientSecret) && sub.LatestInvoice is not null)
        {
            // في v51 الـ Invoice عنده Payments collection بدل PaymentIntent مباشرة
            // جرب نجيب الـ PI من PaymentIntentService
            var invoiceService = new InvoiceService();
            var fullInvoice = await invoiceService.GetAsync(
                sub.LatestInvoiceId!,
                new InvoiceGetOptions { Expand = ["payments"] },
                cancellationToken: ct);

            // من الـ payments collection
            var defaultPayment = fullInvoice.Payments?.Data?
                .FirstOrDefault();

            if (defaultPayment?.Payment?.PaymentIntentId is not null)
            {
                var piService = new PaymentIntentService();
                var pi = await piService.GetAsync(
                    defaultPayment.Payment.PaymentIntentId,
                    cancellationToken: ct);
                clientSecret = pi.ClientSecret ?? string.Empty;
            }
        }

        // 4. PeriodEnd — v48+: من SubscriptionItem
        var periodEnd = sub.Items?.Data?.Count > 0
            ? sub.Items.Data.Max(i => i.CurrentPeriodEnd)
            : DateTime.UtcNow.AddMonths(1);

        _logger.LogInformation(
            "Subscription created: {Id}, PeriodEnd: {End}", sub.Id, periodEnd);

        return new CreateSubscriptionResult(
            sub.Id, customer.Id, clientSecret, periodEnd);
    }

    public bool ValidateWebhookSignature(
        string payload,
        string signature,
        out string eventType,
        out string subscriptionId,
        out DateTime? periodEnd)
    {
        eventType = string.Empty;
        subscriptionId = string.Empty;
        periodEnd = null;

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload, signature, _webhookSecret,
                throwOnApiVersionMismatch: false);

            eventType = stripeEvent.Type;

            if (stripeEvent.Data.Object is Stripe.Subscription sub)
            {
                subscriptionId = sub.Id;

                // v48+: PeriodEnd على SubscriptionItem
                periodEnd = sub.Items?.Data?.Count > 0
                    ? sub.Items.Data.Max(i => i.CurrentPeriodEnd)
                    : null;
            }

            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(
                "Webhook validation failed: {Msg}", ex.Message);
            return false;
        }
    }

    public async Task CancelSubscriptionAsync(
        string            stripeSubscriptionId,
        CancellationToken ct = default)
    {
        var service = new SubscriptionService();
        await service.UpdateAsync(
            stripeSubscriptionId,
            new SubscriptionUpdateOptions { CancelAtPeriodEnd = true },
            cancellationToken: ct);

        _logger.LogInformation(
            "Subscription cancelled at period end: {Id}",
            stripeSubscriptionId);
    }

 

    // ─── Helper ───────────────────────────────────────────────────

 
}