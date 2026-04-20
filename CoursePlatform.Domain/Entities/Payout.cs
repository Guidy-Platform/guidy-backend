using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Payout : AuditableEntity
{
    public Guid InstructorId { get; set; }
    public decimal Amount { get; set; }
    public decimal PlatformFee { get; set; }   // 30% snapshot
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? StripeTransferId { get; set; }   // Stripe Transfer ID
    public string? Notes { get; set; }   // Admin notes
    public DateTime? ProcessedAt { get; set; }   //accept or reject time

    // Navigation
    public AppUser Instructor { get; set; } = null!;
}