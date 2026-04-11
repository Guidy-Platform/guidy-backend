// Application/Features/Auth/Commands/RevokeToken/RevokeTokenCommand.cs
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;