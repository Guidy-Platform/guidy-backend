using FluentValidation;

namespace CoursePlatform.Application.Features.ContactUs.Commands.ReplyToContactMessage;

public class ReplyToContactMessageCommandValidator
    : AbstractValidator<ReplyToContactMessageCommand>
{
    public ReplyToContactMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).GreaterThan(0);
        RuleFor(x => x.Reply)
            .NotEmpty().MaximumLength(5000);
    }
}