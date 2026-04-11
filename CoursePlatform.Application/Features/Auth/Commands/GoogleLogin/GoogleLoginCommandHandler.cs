using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommandHandler
    : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public GoogleLoginCommandHandler(IAuthService authService)
        => _authService = authService;

    public Task<AuthResponseDto> Handle(
        GoogleLoginCommand request, CancellationToken ct)
        => _authService.GoogleLoginAsync(request.IdToken, ct);
}