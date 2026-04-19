using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public string? ActionUrl { get; set; }  // deep link
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }

    // Navigation
    public AppUser User { get; set; } = null!;
}