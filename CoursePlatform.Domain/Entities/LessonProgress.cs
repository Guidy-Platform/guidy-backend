using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public Guid StudentId { get; set; }
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public int WatchedSeconds { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;

    public AppUser Student { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
    public Course Course { get; set; } = null!;
}