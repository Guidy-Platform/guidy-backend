using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponseDto>;