// Domain/Entities/SubCategory.cs
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;

namespace CoursePlatform.Domain.Entities;

public class SubCategory : AuditableEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;  // web-development
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CategoryId { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = [];
}