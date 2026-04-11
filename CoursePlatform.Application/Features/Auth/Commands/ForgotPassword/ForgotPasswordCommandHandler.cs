// Application/Features/Auth/Commands/ForgotPassword/ForgotPasswordCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
        => _authService = authService;

    public async Task<Unit> Handle(
        ForgotPasswordCommand request, CancellationToken ct)
    {
        await _authService.ForgotPasswordAsync(request.Email, ct);
        return Unit.Value;
    }
}