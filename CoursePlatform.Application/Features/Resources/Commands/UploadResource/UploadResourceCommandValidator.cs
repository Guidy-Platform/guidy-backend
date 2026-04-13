using FluentValidation;

namespace CoursePlatform.Application.Features.Resources.Commands.UploadResource;

public class UploadResourceCommandValidator
    : AbstractValidator<UploadResourceCommand>
{
    private const long MaxFileSizeBytes = 100L * 1024 * 1024;  // 100MB

    public UploadResourceCommandValidator()
    {
        RuleFor(x => x.LessonId).GreaterThan(0);
        RuleFor(x => x.SectionId).GreaterThan(0);
        RuleFor(x => x.CourseId).GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Resource title is required.")
            .MaximumLength(200);

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File cannot be empty.")
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("File size cannot exceed 100MB.");
    }
}