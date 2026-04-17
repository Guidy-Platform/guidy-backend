using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Enrollments.Specifications;

public class EnrollmentByStudentAndCourseSpec : BaseSpecification<Enrollment>
{
    public EnrollmentByStudentAndCourseSpec(Guid studentId, int courseId)
        : base(e => e.StudentId == studentId && e.CourseId == courseId)
    { }
}