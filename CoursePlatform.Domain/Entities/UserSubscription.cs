using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class UserSubscription : AuditableEntity
{
    public Guid UserId { get; set; }
    public int PlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }

    // Navigation
    public AppUser User { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;

    // Computed
    public bool IsActive => Status == SubscriptionStatus.Active &&
                            EndDate > DateTime.UtcNow;
}