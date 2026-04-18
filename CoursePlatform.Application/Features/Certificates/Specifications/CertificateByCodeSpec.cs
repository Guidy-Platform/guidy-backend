using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Certificates.Specifications;

public class CertificateByCodeSpec : BaseSpecification<Certificate>
{
    public CertificateByCodeSpec(string verifyCode)
        : base(c => c.VerifyCode == verifyCode)
    {
        ApplyNoTracking();
    }
}