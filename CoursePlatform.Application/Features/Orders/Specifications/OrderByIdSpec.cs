using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Orders.Specifications;

public class OrderByIdSpec : BaseSpecification<Order>
{
    public OrderByIdSpec(int id)
        : base(o => o.Id == id)
    {
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Course");
        ApplyNoTracking();
    }
}