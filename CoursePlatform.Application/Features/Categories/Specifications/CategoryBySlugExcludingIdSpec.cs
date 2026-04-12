using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class CategoryBySlugExcludingIdSpec : BaseSpecification<Category>
{
    public CategoryBySlugExcludingIdSpec(string slug, int excludeId)
        : base(c => c.Slug == slug && c.Id != excludeId) { }
}