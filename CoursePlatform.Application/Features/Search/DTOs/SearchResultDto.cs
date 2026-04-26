using CoursePlatform.Application.Common.Models;
using CoursePlatform.Application.Features.Courses.DTOs;

namespace CoursePlatform.Application.Features.Search.DTOs;

public class SearchResultDto
{
    public int Total { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? Query { get; set; }
    public IReadOnlyList<CourseSummaryDto> Courses { get; set; } = [];

    // Filters applied
    public SearchFiltersDto AppliedFilters { get; set; } = new();
}

public class SearchFiltersDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public string? Level { get; set; }
    public string? Language { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
}