using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Subscriptions.Specifications;

public class AllSubscriptionPlansSpec : BaseSpecification<SubscriptionPlan>
{
    public AllSubscriptionPlansSpec(bool activeOnly = true)
        : base(p => !activeOnly || p.IsActive)
    {
        AddOrderBy(p => p.Price);
        ApplyNoTracking();
    }
}