using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Courses.DTOs;

public class CourseSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double AverageRating { get; set; }  //fill from Reviews Module
    public int TotalStudents { get; set; }  //fill from Enrollment Module
    public DateTime CreatedAt { get; set; }
}