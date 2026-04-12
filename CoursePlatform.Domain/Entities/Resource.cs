using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Resource : AuditableEntity  
{
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;  // pdf, zip, etc.
    public long FileSize { get; set; }  // bytes
    public int LessonId { get; set; }

    // Navigation
    public Lesson Lesson { get; set; } = null!;
}