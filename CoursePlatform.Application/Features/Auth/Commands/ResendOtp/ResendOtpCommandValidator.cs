using FluentValidation;

namespace CoursePlatform.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Purpose)
            .IsInEnum()
            .WithMessage("Purpose must be EmailVerification or PasswordReset.");
    }
}