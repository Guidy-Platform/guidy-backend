using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Coupon : AuditableEntity, ISoftDelete
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; } 
    public int? UsageLimit { get; set; }  // null = unlimited
    public int UsedCount { get; set; } = 0;
    public DateTime? ExpiresAt { get; set; }  // null  not expire
    public bool IsActive { get; set; } = true;

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Computed
    public bool IsExpired
        => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public bool IsUsageLimitReached
        => UsageLimit.HasValue && UsedCount >= UsageLimit.Value;

    public bool CanBeUsed
        => IsActive && !IsDeleted && !IsExpired && !IsUsageLimitReached;

    /// <summary>
    /// calculates the discount amount based on the coupon's type and value for a given order amount
    /// </summary>
    public decimal CalculateDiscount(decimal amount) => DiscountType switch
    {
        DiscountType.Percentage => Math.Round(amount * (DiscountValue / 100), 2),
        DiscountType.FixedAmount => Math.Min(DiscountValue, amount),
        _ => 0
    };
}