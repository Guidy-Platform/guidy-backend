using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Queries.GetCouponById;

public class GetCouponByIdQueryHandler
    : IRequestHandler<GetCouponByIdQuery, CouponDto>
{
    private readonly IUnitOfWork _uow;

    public GetCouponByIdQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CouponDto> Handle(
        GetCouponByIdQuery request, CancellationToken ct)
    {
        var coupon = await _uow.Repository<Coupon>()
                               .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Coupon", request.Id);

        return CouponMapper.ToDto(coupon);
    }
}