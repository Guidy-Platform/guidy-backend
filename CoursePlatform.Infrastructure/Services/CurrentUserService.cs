using CoursePlatform.Application.Contracts.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CoursePlatform.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User
        => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email
        => User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated
        => User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles
        => User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
           ?? Enumerable.Empty<string>();
}