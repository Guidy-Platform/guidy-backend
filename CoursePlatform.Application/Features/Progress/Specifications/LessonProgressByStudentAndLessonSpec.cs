using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Progress.Specifications;

public class LessonProgressByStudentAndLessonSpec
    : BaseSpecification<LessonProgress>
{
    public LessonProgressByStudentAndLessonSpec(Guid studentId, int lessonId)
        : base(lp => lp.StudentId == studentId && lp.LessonId == lessonId)
    { }
}