using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class CoursesByInstructorSpec : BaseSpecification<Course>
{
    public CoursesByInstructorSpec(Guid instructorId)
        : base(c => c.InstructorId == instructorId && !c.IsDeleted)
    {
        ApplyNoTracking();
    }
}