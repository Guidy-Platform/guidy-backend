using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.CreateCoupon;

public record CreateCouponCommand(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    int? UsageLimit,
    DateTime? ExpiresAt
) : IRequest<CouponDto>;