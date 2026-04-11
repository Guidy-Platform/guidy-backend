// Application/Features/Auth/Commands/VerifyEmail/VerifyEmailCommand.cs
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(
    string Email,
    string Code     // ← OTP code
) : IRequest<Unit>;