using FluentValidation;

namespace CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;

public class SendContactMessageCommandValidator
    : AbstractValidator<SendContactMessageCommand>
{
    public SendContactMessageCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(200);

        RuleFor(x => x.Subject)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MinimumLength(10).WithMessage("Message must be at least 10 characters.")
            .MaximumLength(5000);
    }
}