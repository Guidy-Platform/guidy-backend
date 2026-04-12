using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class AllCategoriesWithSubCategoriesSpec : BaseSpecification<Category>
{
    public AllCategoriesWithSubCategoriesSpec()
    {
        AddInclude(c => c.SubCategories);
        AddOrderBy(c => c.Order);
        ApplyNoTracking();
    }
}