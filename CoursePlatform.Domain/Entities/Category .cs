using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;

namespace CoursePlatform.Domain.Entities;

public class Category : AuditableEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;  // tech-and-programming
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int Order { get; set; }  // ترتيب العرض

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public ICollection<SubCategory> SubCategories { get; set; } = [];
}