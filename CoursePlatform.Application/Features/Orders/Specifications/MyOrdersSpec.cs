using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Orders.Specifications;

public class MyOrdersSpec : BaseSpecification<Order>
{
    public MyOrdersSpec(Guid studentId)
        : base(o => o.StudentId == studentId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Course");
        AddOrderByDesc(o => o.CreatedAt);
        ApplyNoTracking();
    }
}