// Application/Contracts/Services/ITokenService.cs
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Contracts.Services;


public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(AppUser user);
    Task<RefreshToken> CreateRefreshTokenAsync(AppUser user);
    Guid? GetUserIdFromExpiredToken(string accessToken);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(
        Guid userId, string token, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(
        RefreshToken token, CancellationToken ct = default);
}