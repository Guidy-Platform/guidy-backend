using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class WishlistItem : BaseEntity
{
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}