using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Subscriptions.Specifications;

public class SubscriptionByStripeIdSpec : BaseSpecification<UserSubscription>
{
    public SubscriptionByStripeIdSpec(string stripeSubscriptionId)
        : base(s => s.StripeSubscriptionId == stripeSubscriptionId)
    {
        AddInclude(s => s.Plan);
    }
}