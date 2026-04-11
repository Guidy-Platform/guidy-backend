// Application/Features/Auth/Commands/VerifyEmail/VerifyEmailCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler
    : IRequestHandler<VerifyEmailCommand, Unit>
{
    private readonly IAuthService _authService;

    public VerifyEmailCommandHandler(IAuthService authService)
        => _authService = authService;

    public async Task<Unit> Handle(
        VerifyEmailCommand request, CancellationToken ct)
    {
        await _authService.VerifyEmailAsync(request.Email, request.Code, ct);
        return Unit.Value;
    }
}