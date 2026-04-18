using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Coupons.Helpers;

public static class CouponMapper
{
    public static CouponDto ToDto(Coupon c) => new()
    {
        Id = c.Id,
        Code = c.Code,
        DiscountType = c.DiscountType.ToString(),
        DiscountValue = c.DiscountValue,
        UsageLimit = c.UsageLimit,
        UsedCount = c.UsedCount,
        RemainingUses = c.UsageLimit.HasValue
                        ? c.UsageLimit.Value - c.UsedCount
                        : null,
        ExpiresAt = c.ExpiresAt,
        IsActive = c.IsActive,
        IsExpired = c.IsExpired,
        CanBeUsed = c.CanBeUsed,
        CreatedAt = c.CreatedAt
    };
}