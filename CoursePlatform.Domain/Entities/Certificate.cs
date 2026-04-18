using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Certificate : AuditableEntity
{
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public string VerifyCode { get; set; } = string.Empty;  // unique GUID
    public string StudentName { get; set; } = string.Empty;  // snapshot
    public string CourseName { get; set; } = string.Empty;  // snapshot
    public string InstructorName { get; set; } = string.Empty;  // snapshot
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}