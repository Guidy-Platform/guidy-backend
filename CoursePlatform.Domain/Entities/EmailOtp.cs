// Domain/Entities/EmailOtp.cs
using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class EmailOtp : BaseEntity
{
    public string Code { get; set; } = string.Empty;  // 6 digits
    public Guid UserId { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;
}