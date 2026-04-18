using CoursePlatform.Application.Features.Coupons.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.GetAllCoupons;

public record GetAllCouponsQuery(
    bool? ActiveOnly = null 
) : IRequest<IReadOnlyList<CouponDto>>;