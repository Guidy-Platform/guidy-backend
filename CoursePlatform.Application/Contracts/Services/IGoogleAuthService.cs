// Application/Contracts/Services/IGoogleAuthService.cs
namespace CoursePlatform.Application.Contracts.Services;

public class GoogleUserPayload
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
}

public interface IGoogleAuthService
{
    Task<GoogleUserPayload?> VerifyTokenAsync(string idToken);
}