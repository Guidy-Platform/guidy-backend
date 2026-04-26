// Infrastructure/Services/GoogleAuthService.cs
using CoursePlatform.Application.Contracts.Services;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoursePlatform.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        IConfiguration config,
        ILogger<GoogleAuthService> logger)
    {
        _clientId = config["Google:ClientId"]!;
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> VerifyIdTokenAsync(
        string idToken, CancellationToken ct = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            };

            var payload = await GoogleJsonWebSignature
                .ValidateAsync(idToken, settings);

            // depend on given name and family name, if not present — use the Name claim
            var firstName = payload.GivenName ?? string.Empty;
            var lastName = payload.FamilyName ?? string.Empty;
            var fullName = payload.Name
                            ?? $"{firstName} {lastName}".Trim();

            // if all things are empty — use the email prefix
            if (string.IsNullOrWhiteSpace(fullName))
                fullName = payload.Email?.Split('@')[0] ?? "User";

            return new GoogleUserInfo(
                GoogleId: payload.Subject,
                Email: payload.Email ?? string.Empty,
                FullName: fullName,
                FirstName: firstName,
                LastName: lastName,
                PictureUrl: payload.Picture);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("Invalid Google token: {Msg}", ex.Message);
            return null;
        }
    }
}