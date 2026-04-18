using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Reviews.Specifications;

public class CourseReviewsSpec : BaseSpecification<Review>
{
    public CourseReviewsSpec(int courseId)
        : base(r => r.CourseId == courseId)
    {
        AddInclude(r => r.Student);
        AddOrderByDesc(r => r.CreatedAt);
        ApplyNoTracking();
    }
}