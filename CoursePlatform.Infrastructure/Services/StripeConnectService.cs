using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace CoursePlatform.Infrastructure.Services;

public class StripeConnectService : IStripeConnectService
{
    private readonly ILogger<StripeConnectService> _logger;
    private readonly string _platformUrl;

    public StripeConnectService(
        IConfiguration config,
        ILogger<StripeConnectService> logger)
    {
        _logger = logger;
        _platformUrl = config["App:BaseUrl"] ?? "https://localhost:7022";
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"]!;
    }

    public async Task<StripeConnectAccountResult> CreateConnectAccountAsync(
        string email, CancellationToken ct = default)
    {
        // إنشاء Stripe Express Account
        var accountOptions = new AccountCreateOptions
        {
            Type = "express",
            Email = email,
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions
                {
                    Requested = true
                }
            }
        };

        var accountService = new AccountService();
        var account = await accountService.CreateAsync(
            accountOptions, cancellationToken: ct);

        // create Onboarding Link
        var linkOptions = new AccountLinkCreateOptions
        {
            Account = account.Id,
            RefreshUrl = $"{_platformUrl}/instructor/stripe/refresh",
            ReturnUrl = $"{_platformUrl}/instructor/stripe/success",
            Type = "account_onboarding"
        };

        var linkService = new AccountLinkService();
        var link = await linkService.CreateAsync(
            linkOptions, cancellationToken: ct);

        _logger.LogInformation(
            "Stripe Connect account created: {AccountId}", account.Id);

        return new StripeConnectAccountResult(account.Id, link.Url);
    }

    public async Task<StripeTransferResult> TransferAsync(
        string stripeAccountId,
        decimal amount,
        string currency,
        int payoutId,
        CancellationToken ct = default)
    {
        try
        {
            var options = new TransferCreateOptions
            {
                Amount = (long)(amount * 100),  // cents
                Currency = currency.ToLower(),
                Destination = stripeAccountId,
                Metadata = new Dictionary<string, string>
                {
                    ["payoutId"] = payoutId.ToString()
                }
            };

            var service = new TransferService();
            var transfer = await service.CreateAsync(
                options, cancellationToken: ct);

            _logger.LogInformation(
                "Stripe Transfer created: {TransferId} for Payout: {PayoutId}",
                transfer.Id, payoutId);

            return new StripeTransferResult(
                transfer.Id, IsSuccess: true, ErrorMessage: null);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe Transfer failed for Payout: {PayoutId}", payoutId);

            return new StripeTransferResult(
                string.Empty, IsSuccess: false, ErrorMessage: ex.Message);
        }
    }
}