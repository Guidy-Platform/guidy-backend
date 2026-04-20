using CoursePlatform.Domain.Constants;
using FluentValidation;

namespace CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;

public class RequestPayoutCommandValidator
    : AbstractValidator<RequestPayoutCommand>
{
    public RequestPayoutCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(PlatformConstants.MinimumPayoutAmount)
            .WithMessage(
                $"Minimum payout amount is ${PlatformConstants.MinimumPayoutAmount}.");
    }
}