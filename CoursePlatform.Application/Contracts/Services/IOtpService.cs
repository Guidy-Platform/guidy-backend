namespace CoursePlatform.Application.Contracts.Services;

public interface IOtpService
{
    Task<string> GenerateAndSaveOtpAsync(
        Guid userId, CancellationToken ct = default);

    Task<bool> ValidateOtpAsync(
        Guid userId, string code, CancellationToken ct = default);
}