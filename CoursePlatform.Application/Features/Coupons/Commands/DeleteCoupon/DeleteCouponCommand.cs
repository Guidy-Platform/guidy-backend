using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.DeleteCoupon;

public record DeleteCouponCommand(int Id) : IRequest<Unit>;