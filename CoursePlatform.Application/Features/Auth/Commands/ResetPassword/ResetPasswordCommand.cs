using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string OtpCode,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Unit>;