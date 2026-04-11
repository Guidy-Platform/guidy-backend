// Infrastructure/Services/TokenService.cs
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoursePlatform.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public TokenService(
        IConfiguration config,
        UserManager<AppUser> userManager,
        AppDbContext context)
    {
        _config = config;
        _userManager = userManager;
        _context = context;
    }

    public async Task<string> CreateAccessTokenAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email,          user.Email!),
            new(ClaimTypes.GivenName,      user.FirstName),
            new(ClaimTypes.Surname,        user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(
                         int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(AppUser user)
    {
        // إلغاء كل الـ tokens القديمة النشطة لنفس الـ user
        var oldTokens = await _context.Set<RefreshToken>()
            .Where(t => t.UserId == user.Id && !t.IsRevoked)
            .ToListAsync();

        foreach (var old in oldTokens)
        {
            old.IsRevoked = true;
            old.RevokedAt = DateTime.UtcNow;
        }

        var refreshToken = new RefreshToken
        {
            Token = GenerateSecureToken(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(
                            int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!))
        };

        await _context.Set<RefreshToken>().AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public Guid? GetUserIdFromExpiredToken(string accessToken)
    {
        var tokenParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,  // نتجاهل الـ expiry هنا
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(_config["Jwt:Key"]!))
        };

        try
        {
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(accessToken, tokenParams, out _);

            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(
        Guid userId, string token, CancellationToken ct = default)
        => await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.Token == token &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, ct);

    public async Task RevokeRefreshTokenAsync(
        RefreshToken token, CancellationToken ct = default)
    {
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}