// Application/Features/Auth/Commands/ChangePassword/ChangePasswordCommandHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandHandler(
        IAuthService authService,
        ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        await _authService.ChangePasswordAsync(
            userId, request.CurrentPassword,
            request.NewPassword, ct);

        return Unit.Value;
    }
}