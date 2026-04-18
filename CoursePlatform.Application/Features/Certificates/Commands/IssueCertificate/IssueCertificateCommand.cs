using CoursePlatform.Application.Features.Certificates.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Commands.IssueCertificate;

public record IssueCertificateCommand(int CourseId) : IRequest<CertificateDto>;