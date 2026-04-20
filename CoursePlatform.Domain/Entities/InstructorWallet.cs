using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class InstructorWallet : AuditableEntity
{
    public Guid InstructorId { get; set; }
    public decimal TotalEarned { get; set; } = 0;  //  Revenue (70%)
    public decimal TotalPaidOut { get; set; } = 0;  //  Payouts completed (Accepted)
    public decimal PendingAmount { get; set; } = 0;  //  Payout Requests not completed
    public decimal AvailableBalance { get; set; } = 0;  // TotalEarned - TotalPaidOut - PendingAmount

    // Stripe Connect Account ID
    public string? StripeAccountId { get; set; }
    public bool IsStripeConnected { get; set; } = false;

    // Navigation
    public AppUser Instructor { get; set; } = null!;
}