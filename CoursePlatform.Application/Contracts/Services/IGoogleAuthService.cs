// Application/Contracts/Services/IGoogleAuthService.cs
namespace CoursePlatform.Application.Contracts.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> VerifyIdTokenAsync(
        string idToken, CancellationToken ct = default);
}

public record GoogleUserInfo(
    string GoogleId,
    string Email,
     string FirstName,
    string LastName,
    string FullName,
    string? PictureUrl
);