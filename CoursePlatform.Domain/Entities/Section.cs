// Domain/Entities/Section.cs
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;

namespace CoursePlatform.Domain.Entities;

public class Section : AuditableEntity, ISoftDelete
{
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int CourseId { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Course Course { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = [];
}