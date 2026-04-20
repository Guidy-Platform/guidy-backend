using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class EnrollmentsByInstructorSpec : BaseSpecification<Enrollment>
{
    public EnrollmentsByInstructorSpec(Guid instructorId)
        : base(e => e.Course.InstructorId == instructorId)
    {
        AddInclude(e => e.Course);
        ApplyNoTracking();
    }
}