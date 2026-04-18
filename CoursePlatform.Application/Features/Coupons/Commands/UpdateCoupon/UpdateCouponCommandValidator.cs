using CoursePlatform.Domain.Enums;
using FluentValidation;

namespace CoursePlatform.Application.Features.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandValidator
    : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Code can only contain letters, numbers, hyphens and underscores.");
        RuleFor(x => x.DiscountType).IsInEnum();
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == DiscountType.Percentage)
            .WithMessage("Percentage discount cannot exceed 100%.");
        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).When(x => x.UsageLimit.HasValue);
        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue);
    }
}