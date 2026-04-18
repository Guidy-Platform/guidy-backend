using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Reviews.Specifications;

public class ReviewByStudentAndCourseSpec : BaseSpecification<Review>
{
    public ReviewByStudentAndCourseSpec(Guid studentId, int courseId)
        : base(r => r.StudentId == studentId && r.CourseId == courseId)
    {
        AddInclude(r => r.Student);
    }
}