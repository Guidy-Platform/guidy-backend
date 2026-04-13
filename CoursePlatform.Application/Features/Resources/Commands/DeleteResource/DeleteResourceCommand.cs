using MediatR;

namespace CoursePlatform.Application.Features.Resources.Commands.DeleteResource;

public record DeleteResourceCommand(
    int ResourceId,
    int LessonId,
    int SectionId,
    int CourseId
) : IRequest<Unit>;