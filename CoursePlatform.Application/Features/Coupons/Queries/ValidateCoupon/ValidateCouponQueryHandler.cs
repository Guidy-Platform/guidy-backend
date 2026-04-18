using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.ValidateCoupon;

public class ValidateCouponQueryHandler
    : IRequestHandler<ValidateCouponQuery, CouponValidationDto>
{
    private readonly IUnitOfWork _uow;

    public ValidateCouponQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CouponValidationDto> Handle(
        ValidateCouponQuery request, CancellationToken ct)
    {
        var spec = new CouponByCodeSpec(request.Code);
        var coupon = await _uow.Repository<Coupon>()
                               .GetEntityWithSpecAsync(spec, ct);

        // مش موجود
        if (coupon is null)
            return Invalid("Coupon code not found.");

        // مش active
        if (!coupon.IsActive)
            return Invalid("This coupon is no longer active.");

        // منتهي الصلاحية
        if (coupon.IsExpired)
            return Invalid("This coupon has expired.");

        // وصل الحد الأقصى
        if (coupon.IsUsageLimitReached)
            return Invalid("This coupon has reached its usage limit.");

        // كل حاجة تمام — احسب الخصم
        var discountAmount = coupon.CalculateDiscount(request.OrderAmount);
        var finalAmount = request.OrderAmount - discountAmount;

        return new CouponValidationDto
        {
            IsValid = true,
            Message = "Coupon is valid.",
            DiscountType = coupon.DiscountType.ToString(),
            DiscountValue = coupon.DiscountValue,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount
        };
    }

    private static CouponValidationDto Invalid(string message) => new()
    {
        IsValid = false,
        Message = message
    };
}