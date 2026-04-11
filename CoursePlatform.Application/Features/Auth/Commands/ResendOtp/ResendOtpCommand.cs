using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ResendOtp;

public enum OtpPurpose
{
    EmailVerification,
    PasswordReset
}

public record ResendOtpCommand(
    string Email,
    OtpPurpose Purpose
) : IRequest<Unit>;