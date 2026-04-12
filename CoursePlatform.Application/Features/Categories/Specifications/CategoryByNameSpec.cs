// Application/Features/Categories/Specifications/CategoryByNameSpec.cs
using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Specifications;

public class CategoryByNameSpec : BaseSpecification<Category>
{
    public CategoryByNameSpec(string name)
        : base(c => c.Name.ToLower() == name.ToLower()) { }
}