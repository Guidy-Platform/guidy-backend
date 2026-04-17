using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Progress.Specifications;

public class LessonProgressByStudentAndCourseSpec
    : BaseSpecification<LessonProgress>
{
    public LessonProgressByStudentAndCourseSpec(Guid studentId, int courseId)
        : base(lp =>
            lp.StudentId == studentId &&
            lp.CourseId == courseId &&
            lp.IsCompleted)
    {
        ApplyNoTracking();
    }
}