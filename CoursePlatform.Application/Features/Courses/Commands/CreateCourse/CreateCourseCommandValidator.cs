using FluentValidation;

namespace CoursePlatform.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator
    : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(50).WithMessage("Description must be at least 50 characters.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500)
            .When(x => x.ShortDescription is not null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be zero or positive.");

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Invalid course level.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.");

        RuleFor(x => x.SubCategoryId)
            .GreaterThan(0).WithMessage("SubCategory is required.");

        RuleFor(x => x.Requirements)
            .NotNull()
            .Must(r => r.Count >= 1)
            .WithMessage("At least one requirement is needed.");

        RuleFor(x => x.WhatYouLearn)
            .NotNull()
            .Must(w => w.Count >= 1)
            .WithMessage("At least one learning objective is needed.");
    }
}