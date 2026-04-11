// Application/Features/Auth/Commands/Login/LoginCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
        => _authService = authService;

    public Task<AuthResponseDto> Handle(
        LoginCommand request, CancellationToken ct)
        => _authService.LoginAsync(request.Email, request.Password, ct);
}