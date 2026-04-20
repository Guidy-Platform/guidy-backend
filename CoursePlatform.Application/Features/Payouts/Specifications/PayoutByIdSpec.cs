// Application/Features/Payouts/Specifications/PayoutByIdSpec.cs
using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class PayoutByIdSpec : BaseSpecification<Payout>
{
    public PayoutByIdSpec(int id)
        : base(p => p.Id == id)
    {
        AddInclude(p => p.Instructor);
    }
}