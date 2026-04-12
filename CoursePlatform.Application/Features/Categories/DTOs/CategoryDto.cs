namespace CoursePlatform.Application.Features.Categories.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int Order { get; set; }
    public int TotalCourses { get; set; }
    public IList<SubCategoryDto> SubCategories { get; set; } = [];
}