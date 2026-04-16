using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Orders.Specifications;

public class CouponByCodeSpec : BaseSpecification<Coupon>
{
    public CouponByCodeSpec(string code)
        : base(c => c.Code.ToLower() == code.ToLower()) { }
}