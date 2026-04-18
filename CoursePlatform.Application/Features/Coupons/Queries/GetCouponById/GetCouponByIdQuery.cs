using CoursePlatform.Application.Features.Coupons.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.GetCouponById;

public record GetCouponByIdQuery(int Id) : IRequest<CouponDto>;