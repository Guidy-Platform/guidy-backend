// Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs
using CoursePlatform.Application.Contracts.Services;
using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Unit>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
        => _authService = authService;

    public async Task<Unit> Handle(
        RegisterCommand request, CancellationToken ct)
    {
        await _authService.RegisterAsync(
            request.FirstName, request.LastName,
            request.Email, request.Password,
            request.Role, ct);

        return Unit.Value;
    }
}