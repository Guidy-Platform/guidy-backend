using FluentValidation;

namespace CoursePlatform.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CourseIds)
            .NotEmpty().WithMessage("At least one course is required.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate courses are not allowed.");

        RuleForEach(x => x.CourseIds)
            .GreaterThan(0);
    }
}