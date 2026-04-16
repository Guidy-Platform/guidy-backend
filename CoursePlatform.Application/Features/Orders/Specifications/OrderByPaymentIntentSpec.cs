using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Orders.Specifications;

public class OrderByPaymentIntentSpec : BaseSpecification<Order>
{
    public OrderByPaymentIntentSpec(string paymentIntentId)
        : base(o => o.PaymentIntentId == paymentIntentId)
    {
        AddInclude(o => o.OrderItems);
    }
}