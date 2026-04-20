using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class AllPayoutsSpec : BaseSpecification<Payout>
{
    public AllPayoutsSpec(PayoutStatus? status = null)
        : base(p => !status.HasValue || p.Status == status.Value)
    {
        AddInclude(p => p.Instructor);
        AddOrderByDesc(p => p.CreatedAt);
        ApplyNoTracking();
    }
}