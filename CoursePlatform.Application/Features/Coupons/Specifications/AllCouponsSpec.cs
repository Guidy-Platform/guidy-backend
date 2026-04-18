using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Coupons.Specifications;

public class AllCouponsSpec : BaseSpecification<Coupon>
{
    public AllCouponsSpec(bool? activeOnly = null)
        : base(c => !activeOnly.HasValue || c.IsActive == activeOnly.Value)
    {
        AddOrderByDesc(c => c.CreatedAt);
        ApplyNoTracking();
    }
}