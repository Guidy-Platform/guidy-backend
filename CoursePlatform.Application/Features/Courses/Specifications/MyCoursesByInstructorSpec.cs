using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Courses.Specifications;

public class MyCoursesByInstructorSpec : BaseSpecification<Course>
{
    public MyCoursesByInstructorSpec(Guid instructorId)
        : base(c => c.InstructorId == instructorId)
    {
        AddInclude(c => c.Instructor);
        AddInclude(c => c.SubCategory);
        AddInclude("SubCategory.Category");
        AddOrderByDesc(c => c.CreatedAt);
        ApplyNoTracking();
    }
}