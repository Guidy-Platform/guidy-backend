using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.InstructorDashboard.Specifications;

public class CompletedOrderItemsByInstructorSpec : BaseSpecification<OrderItem>
{
    public CompletedOrderItemsByInstructorSpec(Guid instructorId)
        : base(i =>
            i.Course.InstructorId == instructorId &&
            i.Order.Status == OrderStatus.Completed)
    {
        AddInclude(i => i.Order);
        ApplyNoTracking();
    }
}