using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Curriculum.Specifications;

public class SectionsByCourseSpec : BaseSpecification<Section>
{
    public SectionsByCourseSpec(int courseId)
        : base(s => s.CourseId == courseId)
    {
        ApplyNoTracking();
    }
}