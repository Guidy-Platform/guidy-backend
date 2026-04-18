// Application/Features/Coupons/Commands/DeleteCoupon/DeleteCouponCommandHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.DeleteCoupon;

public class DeleteCouponCommandHandler
    : IRequestHandler<DeleteCouponCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteCouponCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<Unit> Handle(
        DeleteCouponCommand request, CancellationToken ct)
    {
        var coupon = await _uow.Repository<Coupon>()
                               .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Coupon", request.Id);

        // Soft Delete — AuditInterceptor بيتعامل معاه
        // Orders اللي استخدمت الكوبون هتفضل موجودة بالـ snapshot
        _uow.Repository<Coupon>().Delete(coupon);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}