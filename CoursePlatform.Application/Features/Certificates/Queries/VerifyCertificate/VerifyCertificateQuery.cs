using CoursePlatform.Application.Features.Certificates.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.VerifyCertificate;

public record VerifyCertificateQuery(string VerifyCode)
    : IRequest<CertificateVerifyDto>;