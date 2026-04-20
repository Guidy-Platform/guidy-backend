using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class EnrollmentsByInstructorAndPeriodSpec : BaseSpecification<Enrollment>
{
    public EnrollmentsByInstructorAndPeriodSpec(
        Guid instructorId,
        DateTime from)
        : base(e =>
            e.Course.InstructorId == instructorId &&
            e.EnrolledAt >= from)
    {
        ApplyNoTracking();
    }
}