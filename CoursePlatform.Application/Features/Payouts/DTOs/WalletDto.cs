namespace CoursePlatform.Application.Features.Payouts.DTOs;

public class WalletDto
{
    public decimal TotalEarned { get; set; }
    public decimal TotalPaidOut { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal AvailableBalance { get; set; }
    public bool IsStripeConnected { get; set; }
    public string? StripeAccountId { get; set; }
    public decimal PlatformCommission => TotalEarned > 0
        ? Math.Round(TotalEarned / 0.7m * 0.3m, 2) : 0;
}