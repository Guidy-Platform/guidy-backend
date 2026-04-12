using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class CategoryWithSubCategoriesSpec : BaseSpecification<Category>
{
    public CategoryWithSubCategoriesSpec(int id)
        : base(c => c.Id == id)
    {
        AddInclude(c => c.SubCategories);
        ApplyNoTracking();
    }
}