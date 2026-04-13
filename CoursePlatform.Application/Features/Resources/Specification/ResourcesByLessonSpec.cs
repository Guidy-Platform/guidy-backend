using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Resources.Specifications;

public class ResourcesByLessonSpec : BaseSpecification<Resource>
{
    public ResourcesByLessonSpec(int lessonId)
        : base(r => r.LessonId == lessonId)
    {
        AddOrderBy(r => r.CreatedAt);
        ApplyNoTracking();
    }
}