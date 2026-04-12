using FluentValidation;

namespace CoursePlatform.Application.Features.Courses.Commands.RejectCourse;

public class RejectCourseCommandValidator
    : AbstractValidator<RejectCourseCommand>
{
    public RejectCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000);
    }
}