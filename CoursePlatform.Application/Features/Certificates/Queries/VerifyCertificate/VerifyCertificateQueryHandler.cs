using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Certificates.DTOs;
using CoursePlatform.Application.Features.Certificates.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.VerifyCertificate;

public class VerifyCertificateQueryHandler
    : IRequestHandler<VerifyCertificateQuery, CertificateVerifyDto>
{
    private readonly IUnitOfWork _uow;

    public VerifyCertificateQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CertificateVerifyDto> Handle(
        VerifyCertificateQuery request, CancellationToken ct)
    {
        var spec = new CertificateByCodeSpec(request.VerifyCode);
        var certificate = await _uow.Repository<Certificate>()
                                    .GetEntityWithSpecAsync(spec, ct);

        if (certificate is null)
            return new CertificateVerifyDto { IsValid = false };

        return new CertificateVerifyDto
        {
            IsValid = true,
            StudentName = certificate.StudentName,
            CourseName = certificate.CourseName,
            InstructorName = certificate.InstructorName,
            IssuedAt = certificate.IssuedAt
        };
    }
}