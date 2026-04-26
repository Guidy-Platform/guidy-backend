namespace CoursePlatform.Application.Features.Search.DTOs;

public class SearchSuggestionDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}