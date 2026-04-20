using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class ReviewsByInstructorSpec : BaseSpecification<Review>
{
    public ReviewsByInstructorSpec(Guid instructorId)
        : base(r => r.Course.InstructorId == instructorId)
    {
        ApplyNoTracking();
    }
}