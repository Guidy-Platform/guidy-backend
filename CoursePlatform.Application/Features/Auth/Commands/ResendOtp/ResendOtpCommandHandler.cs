// Application/Features/Auth/Commands/ResendOtp/ResendOtpCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ResendOtp;

public class ResendOtpCommandHandler
    : IRequestHandler<ResendOtpCommand, Unit>
{
    private readonly IAuthService _authService;

    public ResendOtpCommandHandler(IAuthService authService)
        => _authService = authService;

    public async Task<Unit> Handle(
        ResendOtpCommand request, CancellationToken ct)
    {
        await _authService.ResendOtpAsync(
            request.Email, request.Purpose, ct);
        return Unit.Value;
    }
}