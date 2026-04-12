using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class SubCategoryBySlugInCategorySpec : BaseSpecification<SubCategory>
{
    public SubCategoryBySlugInCategorySpec(string slug, int categoryId)
        : base(s => s.Slug == slug && s.CategoryId == categoryId) { }
}