using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Certificates.Specifications;

public class CertificateByStudentAndCourseSpec : BaseSpecification<Certificate>
{
    public CertificateByStudentAndCourseSpec(Guid studentId, int courseId)
        : base(c => c.StudentId == studentId && c.CourseId == courseId)
    { }
}