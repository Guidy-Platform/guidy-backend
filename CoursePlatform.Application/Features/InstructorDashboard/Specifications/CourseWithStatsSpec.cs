using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class CourseWithStatsSpec : BaseSpecification<Course>
{
    public CourseWithStatsSpec(Guid instructorId)
        : base(c => c.InstructorId == instructorId && !c.IsDeleted)
    {
        AddInclude(c => c.Enrollments);
        ApplyNoTracking();
    }
}