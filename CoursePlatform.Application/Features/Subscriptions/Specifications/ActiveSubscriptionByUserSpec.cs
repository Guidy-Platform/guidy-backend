using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Subscriptions.Specifications;

public class ActiveSubscriptionByUserSpec : BaseSpecification<UserSubscription>
{
    public ActiveSubscriptionByUserSpec(Guid userId)
        : base(s =>
            s.UserId == userId &&
            s.Status == SubscriptionStatus.Active &&
            s.EndDate > DateTime.UtcNow)
    {
        AddInclude(s => s.Plan);
        ApplyNoTracking();
    }
}