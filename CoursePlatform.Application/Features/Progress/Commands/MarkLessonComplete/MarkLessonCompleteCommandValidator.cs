using FluentValidation;

namespace CoursePlatform.Application.Features.Progress.Commands.MarkLessonComplete;

public class MarkLessonCompleteCommandValidator
    : AbstractValidator<MarkLessonCompleteCommand>
{
    public MarkLessonCompleteCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.LessonId).GreaterThan(0);
    }
}