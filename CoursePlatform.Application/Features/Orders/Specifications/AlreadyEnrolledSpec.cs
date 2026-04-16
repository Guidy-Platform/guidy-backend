using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Orders.Specifications;

public class AlreadyEnrolledSpec : BaseSpecification<Enrollment>
{
    public AlreadyEnrolledSpec(Guid studentId, int courseId)
        : base(e => e.StudentId == studentId && e.CourseId == courseId)
    { }
}