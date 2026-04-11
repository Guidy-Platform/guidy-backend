using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler
    : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public RevokeTokenCommandHandler(
        IAuthService authService,
        ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        RevokeTokenCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        await _authService.RevokeTokenAsync(
            userId, request.RefreshToken, ct);

        return Unit.Value;
    }
}