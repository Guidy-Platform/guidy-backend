using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Certificates.Specifications;

public class CertificateByIdSpec : BaseSpecification<Certificate>
{
    public CertificateByIdSpec(int id)
        : base(c => c.Id == id)
    {
        AddInclude(c => c.Student);
        AddInclude(c => c.Course);
    }
}