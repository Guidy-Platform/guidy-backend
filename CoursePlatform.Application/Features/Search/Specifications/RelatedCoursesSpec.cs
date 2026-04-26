using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Search.Specifications;

public class RelatedCoursesSpec : BaseSpecification<Course>
{
    public RelatedCoursesSpec(int subCategoryId, int excludeCourseId, int take = 6)
        : base(c =>
            c.Status == CourseStatus.Published &&
            c.SubCategoryId == subCategoryId &&
            c.Id != excludeCourseId)
    {
        AddInclude(c => c.Instructor);
        AddInclude(c => c.SubCategory);
        AddOrderByDesc(c => c.AverageRating);
        ApplyPaging(1, take);
        ApplyNoTracking();
    }
}