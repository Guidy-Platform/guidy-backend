using CoursePlatform.Application.Contracts.Services;

namespace CoursePlatform.Infrastructure.Services;

public class MockStripeConnectService : IStripeConnectService
{
    public Task<StripeConnectAccountResult> CreateConnectAccountAsync(
        string email, CancellationToken ct = default)
    {
        var fakeAccountId = $"acct_test_{Guid.NewGuid():N}"[..20];
        var fakeOnboardingUrl = $"https://connect.stripe.com/test/onboard/{fakeAccountId}";

        return Task.FromResult(
            new StripeConnectAccountResult(fakeAccountId, fakeOnboardingUrl));
    }

    public Task<StripeTransferResult> TransferAsync(
        string stripeAccountId, decimal amount,
        string currency, int payoutId,
        CancellationToken ct = default)
    {
        var fakeTransferId = $"tr_test_{Guid.NewGuid():N}"[..20];

        return Task.FromResult(
            new StripeTransferResult(fakeTransferId, IsSuccess: true, null));
    }
}