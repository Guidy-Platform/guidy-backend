using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.ToggleCoupon;

public class ToggleCouponCommandHandler
    : IRequestHandler<ToggleCouponCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public ToggleCouponCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<bool> Handle(
        ToggleCouponCommand request, CancellationToken ct)
    {
        var coupon = await _uow.Repository<Coupon>()
                               .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Coupon", request.Id);

        coupon.IsActive = !coupon.IsActive;

        _uow.Repository<Coupon>().Update(coupon);
        await _uow.CompleteAsync(ct);

        return coupon.IsActive;
    }
}