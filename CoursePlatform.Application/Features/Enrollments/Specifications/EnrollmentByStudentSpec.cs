using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Enrollments.Specifications;

public class EnrollmentByStudentSpec : BaseSpecification<Enrollment>
{
    public EnrollmentByStudentSpec(Guid studentId)
        : base(e => e.StudentId == studentId)
    {
        AddInclude(e => e.Course);
        AddInclude("Course.SubCategory");
        AddOrderByDesc(e => e.EnrolledAt);
        ApplyNoTracking();
    }
}