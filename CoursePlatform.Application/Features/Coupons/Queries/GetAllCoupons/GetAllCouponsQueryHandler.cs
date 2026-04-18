using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Helpers;
using CoursePlatform.Application.Features.Coupons.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler
    : IRequestHandler<GetAllCouponsQuery, IReadOnlyList<CouponDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllCouponsQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<IReadOnlyList<CouponDto>> Handle(
        GetAllCouponsQuery request, CancellationToken ct)
    {
        var spec = new AllCouponsSpec(request.ActiveOnly);
        var coupons = await _uow.Repository<Coupon>()
                                .GetAllWithSpecAsync(spec, ct);

        return coupons.Select(CouponMapper.ToDto).ToList();
    }
}