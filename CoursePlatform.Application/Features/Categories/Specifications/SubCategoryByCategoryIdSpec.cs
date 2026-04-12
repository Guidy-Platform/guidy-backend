using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;


namespace CoursePlatform.Application.Features.Categories.Specifications
{
    public class SubCategoryByCategoryIdSpec : BaseSpecification<SubCategory>
    {
        public SubCategoryByCategoryIdSpec(int categoryId)
            : base(sc => sc.CategoryId == categoryId)
        {
        }
    }
}
