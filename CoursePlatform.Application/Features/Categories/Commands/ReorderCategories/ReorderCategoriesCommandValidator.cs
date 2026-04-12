using FluentValidation;

namespace CoursePlatform.Application.Features.Categories.Commands.ReorderCategories;

public class ReorderCategoriesCommandValidator
    : AbstractValidator<ReorderCategoriesCommand>
{
    public ReorderCategoriesCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Items list cannot be empty.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Id).GreaterThan(0);
            item.RuleFor(x => x.Order).GreaterThan(0);
        });

        // لازم الـ orders تكون unique
        RuleFor(x => x.Items)
            .Must(items =>
                items.Select(i => i.Order).Distinct().Count() == items.Count)
            .WithMessage("Order values must be unique.");
    }
}