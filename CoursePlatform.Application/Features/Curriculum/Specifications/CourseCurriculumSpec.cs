using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Curriculum.Specifications;

public class CourseCurriculumSpec : BaseSpecification<Course>
{
    public CourseCurriculumSpec(int courseId)
        : base(c => c.Id == courseId)
    {
        AddInclude(c => c.Instructor);
        AddInclude("Sections");
        AddInclude("Sections.Lessons");
        AddInclude("Sections.Lessons.Resources");
        ApplyNoTracking();
    }
}