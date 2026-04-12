using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Courses.Specifications;

public class PendingCoursesSpec : BaseSpecification<Course>
{
    public PendingCoursesSpec()
        : base(c => c.Status == CourseStatus.UnderReview)
    {
        AddInclude(c => c.Instructor);
        AddInclude(c => c.SubCategory);
        AddInclude("SubCategory.Category");
        AddOrderBy(c => c.UpdatedAt!);
        ApplyNoTracking();
    }
}