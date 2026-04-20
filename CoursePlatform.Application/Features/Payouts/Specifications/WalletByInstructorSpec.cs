using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Payouts.Specifications;

public class WalletByInstructorSpec : BaseSpecification<InstructorWallet>
{
    public WalletByInstructorSpec(Guid instructorId)
        : base(w => w.InstructorId == instructorId) { }
}