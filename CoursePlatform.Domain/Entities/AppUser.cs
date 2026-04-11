using CoursePlatform.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Domain.Entities;

public class AppUser : IdentityUser<Guid>, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Computed 
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties fill it with each feature
    // public ICollection<Course> Courses { get; set; } = [];
    // public ICollection<Enrollment> Enrollments { get; set; } = [];
}