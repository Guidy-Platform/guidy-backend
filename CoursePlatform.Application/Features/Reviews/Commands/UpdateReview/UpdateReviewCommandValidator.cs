using FluentValidation;

namespace CoursePlatform.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandValidator
    : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).GreaterThan(0);
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Comment)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(2000);
    }
}