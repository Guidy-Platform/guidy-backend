using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Courses.DTOs;

public class CourseQueryParams
{
    private const int MaxPageSize = 30;
    private int _pageSize = 12;

    public int PageIndex { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? Search { get; set; }
    public int? SubCategoryId { get; set; }
    public int? CategoryId { get; set; }
    public CourseLevel? Level { get; set; }
    public string? Language { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    // "newest" | "price_asc" | "price_desc" | "rating"
}