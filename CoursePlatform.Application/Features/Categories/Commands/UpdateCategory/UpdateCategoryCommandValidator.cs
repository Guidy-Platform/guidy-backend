// Application/Features/Categories/Commands/UpdateCategory/UpdateCategoryCommandValidator.cs
using FluentValidation;

namespace CoursePlatform.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator
    : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500)
            .When(x => x.Description is not null);
    }
}