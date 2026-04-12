// Application/Features/Categories/Specifications/SubCategoryByNameInCategorySpec.cs
using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class SubCategoryByNameInCategorySpec : BaseSpecification<SubCategory>
{
    public SubCategoryByNameInCategorySpec(string name, int categoryId)
        : base(s =>
            s.Name.ToLower() == name.ToLower() &&
            s.CategoryId == categoryId)
    { }
}