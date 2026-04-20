using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class EnrollmentByStudentSpec : BaseSpecification<Enrollment>
{
    public EnrollmentByStudentSpec(Guid studentId)
        : base(e => e.StudentId == studentId) { }
}