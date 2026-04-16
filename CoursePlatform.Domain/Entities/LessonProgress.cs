using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public Guid StudentId { get; set; }
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
    public Course Course { get; set; } = null!;
}