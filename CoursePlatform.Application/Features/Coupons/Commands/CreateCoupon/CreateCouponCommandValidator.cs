using FluentValidation;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandValidator
    : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MinimumLength(3).WithMessage("Code must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Code can only contain letters, numbers, hyphens and underscores.");

        RuleFor(x => x.DiscountType)
            .IsInEnum().WithMessage("Invalid discount type.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than 0.");

        // لو Percentage — مش ممكن يتعدى 100
        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == DiscountType.Percentage)
            .WithMessage("Percentage discount cannot exceed 100%.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue)
            .WithMessage("Usage limit must be greater than 0.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiry date must be in the future.");
    }
}