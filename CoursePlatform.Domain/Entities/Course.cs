using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Interfaces;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Course : AuditableEntity, ISoftDelete
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public string Language { get; set; } = "English";
    public string? Requirements { get; set; }  // JSON array as string
    public string? WhatYouLearn { get; set; }  // JSON array as string
    public string? RejectionReason { get; set; }  // Admin rejection note
    public double AverageRating { get; set; } = 0;
    public int TotalRatings { get; set; } = 0;

    // FK
    public Guid InstructorId { get; set; }
    public int SubCategoryId { get; set; }

    // ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public AppUser Instructor { get; set; } = null!;
    public SubCategory SubCategory { get; set; } = null!;


    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}