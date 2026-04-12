using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Curriculum.Specifications;

public class LessonsBySectionSpec : BaseSpecification<Lesson>
{
    public LessonsBySectionSpec(int sectionId)
        : base(l => l.SectionId == sectionId)
    {
        ApplyNoTracking();
    }
}