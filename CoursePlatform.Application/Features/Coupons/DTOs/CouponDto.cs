using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Coupons.DTOs;

public class CouponDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public int? RemainingUses { get; set; }  // null = unlimited
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public bool CanBeUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}