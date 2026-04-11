// Application/Features/Auth/Commands/Login/LoginCommand.cs
using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;