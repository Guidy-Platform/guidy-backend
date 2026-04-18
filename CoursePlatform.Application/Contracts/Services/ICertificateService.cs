using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Contracts.Services;

public interface ICertificateService
{
    
    Task<byte[]> GeneratePdfAsync(
        Certificate certificate,
        CancellationToken ct = default);
}