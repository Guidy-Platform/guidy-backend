using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Lesson : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationInSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; } = false;
    public LessonType Type { get; set; } = LessonType.Video;
    public int SectionId { get; set; }

 

    // Navigation
    public Section Section { get; set; } = null!;
    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}