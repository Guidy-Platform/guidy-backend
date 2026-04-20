using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class AllEnrollmentsSpec : BaseSpecification<Enrollment>
{
    public AllEnrollmentsSpec() { ApplyNoTracking(); }
}