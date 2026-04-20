using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class CompletedPayoutsByInstructorSpec : BaseSpecification<Payout>
{
    public CompletedPayoutsByInstructorSpec(Guid instructorId)
        : base(p => p.InstructorId == instructorId &&
                    p.Status == PayoutStatus.Approved)
    {
        ApplyNoTracking();
    }
}