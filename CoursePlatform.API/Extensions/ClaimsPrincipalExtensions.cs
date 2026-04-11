// API/Extensions/ClaimsPrincipalExtensions.cs
using System.Security.Claims;

namespace CoursePlatform.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID claim not found.");
        return Guid.Parse(value);
    }

    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email)
           ?? throw new UnauthorizedAccessException("Email claim not found.");

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
        => user.FindAll(ClaimTypes.Role).Select(c => c.Value);
}