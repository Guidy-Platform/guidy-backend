using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class CoursesByInstructorCountSpec : BaseSpecification<Course>
{
    public CoursesByInstructorCountSpec(Guid instructorId)
        : base(c => c.InstructorId == instructorId && !c.IsDeleted) { }
}