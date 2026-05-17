using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Courses.Specifications;

public class AllCoursesFilterSpec : BaseSpecification<Course>
{
    public AllCoursesFilterSpec(string? search = null)
        : base(c =>
            !c.IsDeleted &&
            (string.IsNullOrEmpty(search) ||
             c.Title.ToLower().Contains(search.ToLower())))
    {
        AddOrderBy(c => c.Title);
        ApplyNoTracking();
    }
}