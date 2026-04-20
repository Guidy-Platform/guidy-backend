using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Admin.Specifications;

public class AllReviewsSpec : BaseSpecification<Review>
{
    public AllReviewsSpec() { ApplyNoTracking(); }
}