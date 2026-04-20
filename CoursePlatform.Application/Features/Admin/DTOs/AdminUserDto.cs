namespace CoursePlatform.Application.Features.Admin.DTOs;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<string> Roles { get; set; } = [];

    // Stats
    public int EnrollmentsCount { get; set; }
    public int CoursesCount { get; set; }
}