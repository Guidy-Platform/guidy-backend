// Application/Features/Courses/DTOs/CourseDto.cs
namespace CoursePlatform.Application.Features.Courses.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public List<string> Requirements { get; set; } = [];
    public List<string> WhatYouLearn { get; set; } = [];
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int SubCategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalStudents { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}