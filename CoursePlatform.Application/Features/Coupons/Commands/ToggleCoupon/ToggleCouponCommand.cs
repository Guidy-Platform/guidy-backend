using MediatR;

namespace CoursePlatform.Application.Features.Coupons.Commands.ToggleCoupon;

public record ToggleCouponCommand(int Id) : IRequest<bool>;
