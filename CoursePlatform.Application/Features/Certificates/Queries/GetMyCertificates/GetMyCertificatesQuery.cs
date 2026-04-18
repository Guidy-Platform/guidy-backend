using CoursePlatform.Application.Features.Certificates.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.GetMyCertificates;

public record GetMyCertificatesQuery : IRequest<IReadOnlyList<CertificateDto>>;