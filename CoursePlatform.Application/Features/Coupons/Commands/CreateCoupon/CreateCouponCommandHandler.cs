using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Helpers;
using CoursePlatform.Application.Features.Coupons.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler
    : IRequestHandler<CreateCouponCommand, CouponDto>
{
    private readonly IUnitOfWork _uow;

    public CreateCouponCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CouponDto> Handle(
        CreateCouponCommand request, CancellationToken ct)
    {
        // تحقق من uniqueness الـ code (case-insensitive)
        var existingSpec = new CouponByCodeSpec(request.Code);
        var exists = await _uow.Repository<Coupon>()
                                     .AnyAsync(existingSpec, ct);
        if (exists)
            throw new ConflictException(
                $"Coupon code '{request.Code.ToUpper()}' already exists.");

        var coupon = new Coupon
        {
            Code = request.Code.ToUpper(),  // نحفظه uppercase دايماً
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            UsageLimit = request.UsageLimit,
            ExpiresAt = request.ExpiresAt,
            IsActive = true
        };

        await _uow.Repository<Coupon>().AddAsync(coupon, ct);
        await _uow.CompleteAsync(ct);

        return CouponMapper.ToDto(coupon);
    }
}