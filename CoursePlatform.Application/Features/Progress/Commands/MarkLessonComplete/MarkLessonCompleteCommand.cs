using MediatR;

namespace CoursePlatform.Application.Features.Progress.Commands.MarkLessonComplete;

public record MarkLessonCompleteCommand(
    int CourseId,
    int LessonId
) : IRequest<MarkLessonCompleteResult>;

public record MarkLessonCompleteResult(
    int LessonId,
    bool AlreadyCompleted,
    double NewProgressPercent,
    bool CourseCompleted
);