using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class CategoryBySlugSpec : BaseSpecification<Category>
{
    public CategoryBySlugSpec(string slug)
        : base(c => c.Slug == slug) { }
}