using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class PayoutsByInstructorSpec : BaseSpecification<Payout>
{
    public PayoutsByInstructorSpec(Guid instructorId)
        : base(p => p.InstructorId == instructorId)
    {
        AddInclude(p => p.Instructor);
        AddOrderByDesc(p => p.CreatedAt);
        ApplyNoTracking();
    }
}