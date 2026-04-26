using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Search.Specifications;

public class TrendingCoursesSpec : BaseSpecification<Course>
{
    public TrendingCoursesSpec(int take = 10)
        : base(c =>
            c.Status == CourseStatus.Published &&
            c.CreatedAt >= DateTime.UtcNow.AddDays(-30))
    {
        AddInclude(c => c.Instructor);
        AddInclude(c => c.SubCategory);
        AddInclude(c => c.Enrollments);
        AddOrderByDesc(c => c.Enrollments.Count);
        ApplyPaging(1, take);
        ApplyNoTracking();
    }
}