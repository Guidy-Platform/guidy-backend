using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ReorderLessons;

public record LessonOrderItem(int LessonId, int Order);

public record ReorderLessonsCommand(
    int CourseId,
    int SectionId,
    IList<LessonOrderItem> Items
) : IRequest<Unit>;