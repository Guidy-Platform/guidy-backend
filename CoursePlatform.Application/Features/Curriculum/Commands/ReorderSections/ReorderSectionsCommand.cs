using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ReorderSections;

public record SectionOrderItem(int SectionId, int Order);

public record ReorderSectionsCommand(
    int CourseId,
    IList<SectionOrderItem> Items
) : IRequest<Unit>;