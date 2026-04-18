using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Coupons.Specifications;

public class CouponByCodeSpec : BaseSpecification<Coupon>
{
    // case-insensitive + ignore soft deleted
    public CouponByCodeSpec(string code)
        : base(c => c.Code.ToLower() == code.ToLower()) { }
}