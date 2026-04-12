using FluentValidation;

namespace CoursePlatform.Application.Features.Curriculum.Commands.ReorderSections;

public class ReorderSectionsCommandValidator
    : AbstractValidator<ReorderSectionsCommand>
{
    public ReorderSectionsCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.SectionId).GreaterThan(0);
            item.RuleFor(x => x.Order).GreaterThan(0);
        });
        RuleFor(x => x.Items)
            .Must(items =>
                items.Select(i => i.Order).Distinct().Count() == items.Count)
            .WithMessage("Order values must be unique.");
    }
}