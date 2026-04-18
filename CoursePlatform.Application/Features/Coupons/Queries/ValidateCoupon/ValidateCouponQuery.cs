using CoursePlatform.Application.Features.Coupons.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.ValidateCoupon;

public record ValidateCouponQuery(
    string Code,
    decimal OrderAmount 
) : IRequest<CouponValidationDto>;