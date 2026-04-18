using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Reviews.Specifications;

public class ReviewByIdSpec : BaseSpecification<Review>
{
    public ReviewByIdSpec(int id)
        : base(r => r.Id == id)
    {
        AddInclude(r => r.Student);
    }
}