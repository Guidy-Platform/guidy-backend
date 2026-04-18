using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Certificates.Commands.IssueCertificate;
using CoursePlatform.Application.Features.Certificates.DTOs;
using CoursePlatform.Application.Features.Certificates.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.GetMyCertificates;

public class GetMyCertificatesQueryHandler
    : IRequestHandler<GetMyCertificatesQuery, IReadOnlyList<CertificateDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyCertificatesQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CertificateDto>> Handle(
        GetMyCertificatesQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new CertificatesByStudentSpec(studentId);
        var certificates = await _uow.Repository<Certificate>()
                                     .GetAllWithSpecAsync(spec, ct);

        // BaseUrl from ICurrentUserService direct 
        return certificates
            .Select(c => IssueCertificateCommandHandler
                .MapToDto(c, _currentUser.BaseUrl))
            .ToList();
    }
}