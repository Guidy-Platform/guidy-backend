namespace CoursePlatform.Application.Features.Categories.DTOs;

public class SubCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int CourseCount { get; set; }  // will fill later   
}