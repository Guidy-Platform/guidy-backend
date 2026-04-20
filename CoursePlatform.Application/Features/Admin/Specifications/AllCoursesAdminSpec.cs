using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class AllCoursesAdminSpec : BaseSpecification<Course>
{
    public AllCoursesAdminSpec(
        CourseStatus? status = null,
        string? search = null)
        : base(c =>
            !c.IsDeleted &&
            (!status.HasValue || c.Status == status.Value) &&
            (string.IsNullOrEmpty(search) ||
                c.Title.ToLower().Contains(search.ToLower())))
    {
        AddInclude(c => c.Instructor);
        AddInclude(c => c.Enrollments);
        AddOrderByDesc(c => c.CreatedAt);
        ApplyNoTracking();
    }
}