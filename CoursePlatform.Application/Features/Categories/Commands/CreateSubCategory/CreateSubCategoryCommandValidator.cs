using FluentValidation;

namespace CoursePlatform.Application.Features.Categories.Commands.CreateSubCategory;

public class CreateSubCategoryCommandValidator
    : AbstractValidator<CreateSubCategoryCommand>
{
    public CreateSubCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500)
            .When(x => x.Description is not null);
    }
}