using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Subscriptions.Specifications;

public class SubscriptionByUserSpec : BaseSpecification<UserSubscription>
{
    public SubscriptionByUserSpec(Guid userId)
        : base(s => s.UserId == userId)
    {
        AddInclude(s => s.Plan);
        AddOrderByDesc(s => s.CreatedAt);
        ApplyNoTracking();
    }
}