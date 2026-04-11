using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
        => _authService = authService;

    public Task<AuthResponseDto> Handle(
        RefreshTokenCommand request, CancellationToken ct)
        => _authService.RefreshTokenAsync(
            request.AccessToken, request.RefreshToken, ct);
}