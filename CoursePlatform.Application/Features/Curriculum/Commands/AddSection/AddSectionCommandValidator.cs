using FluentValidation;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddSection;

public class AddSectionCommandValidator
    : AbstractValidator<AddSectionCommand>
{
    public AddSectionCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Section title is required.")
            .MaximumLength(200);
    }
}