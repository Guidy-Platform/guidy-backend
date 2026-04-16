using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public int OrderId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Order Order { get; set; } = null!;
}