using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class CompletedOrderItemsByInstructorAndPeriodSpec
    : BaseSpecification<OrderItem>
{
    public CompletedOrderItemsByInstructorAndPeriodSpec(
        Guid instructorId,
        DateTime from)
        : base(i =>
            i.Course.InstructorId == instructorId &&
            i.Order.Status == OrderStatus.Completed &&
            i.Order.PaidAt >= from)
    {
        AddInclude(i => i.Order);
        ApplyNoTracking();
    }
}