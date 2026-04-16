using FluentValidation;

namespace CoursePlatform.Application.Features.Orders.Commands.ApplyCoupon;

public class ApplyCouponCommandValidator
    : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.CouponCode).NotEmpty().MaximumLength(50);
    }
}