namespace CoursePlatform.Application.Contracts.Services;

public record StripeConnectAccountResult(
    string AccountId,
    string OnboardingUrl
);

public record StripeTransferResult(
    string TransferId,
    bool IsSuccess,
    string? ErrorMessage
);

public interface IStripeConnectService
{
    /// <summary>create Stripe Connect account for Instructor</summary>
    Task<StripeConnectAccountResult> CreateConnectAccountAsync(
        string email, CancellationToken ct = default);

    /// <summary>create Transfer for Instructor's Stripe account</summary>
    Task<StripeTransferResult> TransferAsync(
        string stripeAccountId,
        decimal amount,
        string currency,
        int payoutId,
        CancellationToken ct = default);
}