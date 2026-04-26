using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Search.Specifications;

public class SuggestionsSpec : BaseSpecification<Course>
{
    public SuggestionsSpec(string query, int take = 5)
        : base(c =>
            c.Status == CourseStatus.Published &&
            c.Title.ToLower().Contains(query.ToLower()))
    {
        AddInclude("SubCategory.Category");
        AddOrderByDesc(c => c.AverageRating);
        ApplyPaging(1, take);
        ApplyNoTracking();
    }
}