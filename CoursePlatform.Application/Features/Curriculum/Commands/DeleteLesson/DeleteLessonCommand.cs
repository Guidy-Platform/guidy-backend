using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.DeleteLesson;

public record DeleteLessonCommand(
    int LessonId,
    int SectionId,
    int CourseId
) : IRequest<Unit>;