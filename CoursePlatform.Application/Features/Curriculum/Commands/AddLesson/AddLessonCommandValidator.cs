using CoursePlatform.Domain.Enums;
using FluentValidation;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddLesson;

public class AddLessonCommandValidator
    : AbstractValidator<AddLessonCommand>
{
    public AddLessonCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.SectionId).GreaterThan(0);
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.DurationInSeconds)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.VideoUrl)
            .NotEmpty().WithMessage("Video URL is required for video lessons.")
            .When(x => x.Type == LessonType.Video);
    }
}