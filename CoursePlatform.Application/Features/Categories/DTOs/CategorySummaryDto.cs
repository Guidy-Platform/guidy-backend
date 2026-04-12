namespace CoursePlatform.Application.Features.Categories.DTOs;

public class CategorySummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int SubCategoryCount { get; set; }
    public int TotalCourses { get; set; }
}