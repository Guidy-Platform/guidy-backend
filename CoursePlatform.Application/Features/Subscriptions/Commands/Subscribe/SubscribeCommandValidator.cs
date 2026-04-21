using FluentValidation;

namespace CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;

public class SubscribeCommandValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeCommandValidator()
    {
        RuleFor(x => x.PlanId).GreaterThan(0);
    }
}