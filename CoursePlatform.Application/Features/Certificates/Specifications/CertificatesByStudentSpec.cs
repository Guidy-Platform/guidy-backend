using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Certificates.Specifications;

public class CertificatesByStudentSpec : BaseSpecification<Certificate>
{
    public CertificatesByStudentSpec(Guid studentId)
        : base(c => c.StudentId == studentId)
    {
        AddOrderByDesc(c => c.IssuedAt);
        ApplyNoTracking();
    }
}