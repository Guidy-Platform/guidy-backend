using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Coupons.Specifications;

public class CouponByIdSpec : BaseSpecification<Coupon>
{
    public CouponByIdSpec(int id)
        : base(c => c.Id == id) { }
}