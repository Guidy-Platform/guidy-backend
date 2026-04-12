using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ToggleFreePreview;

public record ToggleFreePreviewCommand(
    int LessonId,
    int SectionId,
    int CourseId
) : IRequest<bool>;   // ← يرجع الـ state الجديد