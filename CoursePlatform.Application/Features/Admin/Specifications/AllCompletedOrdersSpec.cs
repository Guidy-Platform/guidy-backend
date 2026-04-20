using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class AllCompletedOrdersSpec : BaseSpecification<Order>
{
    public AllCompletedOrdersSpec()
        : base(o => o.Status == OrderStatus.Completed)
    {
        ApplyNoTracking();
    }
}