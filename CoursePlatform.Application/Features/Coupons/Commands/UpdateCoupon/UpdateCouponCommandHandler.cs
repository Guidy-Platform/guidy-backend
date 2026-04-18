using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Helpers;
using CoursePlatform.Application.Features.Coupons.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandHandler
    : IRequestHandler<UpdateCouponCommand, CouponDto>
{
    private readonly IUnitOfWork _uow;

    public UpdateCouponCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CouponDto> Handle(
        UpdateCouponCommand request, CancellationToken ct)
    {
        var coupon = await _uow.Repository<Coupon>()
                               .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Coupon", request.Id);

        // لو الـ code اتغير — تحقق من uniqueness
        if (!string.Equals(coupon.Code, request.Code,
                StringComparison.OrdinalIgnoreCase))
        {
            var existingSpec = new CouponByCodeSpec(request.Code);
            var exists = await _uow.Repository<Coupon>()
                                         .AnyAsync(existingSpec, ct);
            if (exists)
                throw new ConflictException(
                    $"Coupon code '{request.Code.ToUpper()}' already exists.");
        }

        // مش ممكن تقلل الـ UsageLimit لأقل من الـ UsedCount
        if (request.UsageLimit.HasValue &&
            request.UsageLimit.Value < coupon.UsedCount)
            throw new BadRequestException(
                $"Usage limit cannot be less than current used count ({coupon.UsedCount}).");

        coupon.Code = request.Code.ToUpper();
        coupon.DiscountType = request.DiscountType;
        coupon.DiscountValue = request.DiscountValue;
        coupon.UsageLimit = request.UsageLimit;
        coupon.ExpiresAt = request.ExpiresAt;

        _uow.Repository<Coupon>().Update(coupon);
        await _uow.CompleteAsync(ct);

        return CouponMapper.ToDto(coupon);
    }
}