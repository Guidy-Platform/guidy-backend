namespace CoursePlatform.Application.Contracts.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }   // Guid 
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}