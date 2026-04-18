using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Certificates.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Certificates.Queries.GetCertificateById;

public class GetCertificateByIdQueryHandler
    : IRequestHandler<GetCertificateByIdQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ICertificateService _certService;

    public GetCertificateByIdQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        ICertificateService certService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _certService = certService;
    }

    public async Task<byte[]> Handle(
        GetCertificateByIdQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new CertificateByIdSpec(request.CertificateId);
        var certificate = await _uow.Repository<Certificate>()
                                    .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Certificate", request.CertificateId);
        
        // check if the current user is the owner of the certificate or an admin
        if (certificate.StudentId != studentId &&
            !_currentUser.Roles.Contains("Admin"))
            throw new ForbiddenException();

        return await _certService.GeneratePdfAsync(certificate, ct);
    }
}