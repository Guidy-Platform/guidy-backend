// Application/Features/Auth/Commands/ResendOtp/ResendOtpCommand.cs
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.ResendOtp;

public record ResendOtpCommand(string Email) : IRequest<Unit>;