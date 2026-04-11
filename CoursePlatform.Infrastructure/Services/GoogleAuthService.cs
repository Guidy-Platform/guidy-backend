// Infrastructure/Services/GoogleAuthService.cs
using CoursePlatform.Application.Contracts.Services;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace CoursePlatform.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _config;

    public GoogleAuthService(IConfiguration config)
        => _config = config;

    public async Task<GoogleUserPayload?> VerifyTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_config["Google:ClientId"]!]
            };

            var payload = await GoogleJsonWebSignature
                .ValidateAsync(idToken, settings);

            return new GoogleUserPayload
            {
                Email = payload.Email,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName
            };
        }
        catch
        {
            return null;  // Invalid token
        }
    }
}