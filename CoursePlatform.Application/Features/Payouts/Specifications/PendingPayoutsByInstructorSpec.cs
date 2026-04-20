using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class PendingPayoutsByInstructorSpec : BaseSpecification<Payout>
{
    public PendingPayoutsByInstructorSpec(Guid instructorId)
        : base(p => p.InstructorId == instructorId &&
                    p.Status == PayoutStatus.Pending)
    {
        ApplyNoTracking();
    }
}