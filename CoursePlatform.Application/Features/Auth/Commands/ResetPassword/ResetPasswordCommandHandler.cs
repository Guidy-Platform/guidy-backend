// Application/Features/Auth/Commands/ResetPassword/ResetPasswordCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
        => _authService = authService;

    public async Task<Unit> Handle(
        ResetPasswordCommand request, CancellationToken ct)
    {
        await _authService.ResetPasswordAsync(
            request.Email,
            request.OtpCode,
            request.NewPassword,
            ct);

        return Unit.Value;
    }
}