using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Lesson : AuditableEntity, ISoftDelete
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationInSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; } = false;
    public LessonType Type { get; set; } = LessonType.Video;
    public int SectionId { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Section Section { get; set; } = null!;
    public ICollection<Resource> Resources { get; set; } = [];
}