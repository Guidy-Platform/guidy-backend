using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.GetCertificateById;

public record GetCertificateByIdQuery(int CertificateId) : IRequest<byte[]>;