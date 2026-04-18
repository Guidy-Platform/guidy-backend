using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Review : AuditableEntity
{
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public int Rating { get; set; }   // 1 → 5
    public string Comment { get; set; } = string.Empty;

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}