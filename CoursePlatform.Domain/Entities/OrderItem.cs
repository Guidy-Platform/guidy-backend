using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;  // snapshot
    public decimal Price { get; set; }                  // snapshot

    // Navigation
    public Order Order { get; set; } = null!;
    public Course Course { get; set; } = null!;
}