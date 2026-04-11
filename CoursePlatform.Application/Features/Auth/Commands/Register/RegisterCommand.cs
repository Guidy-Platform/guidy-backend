using MediatR;

namespace CoursePlatform.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string Role
) : IRequest<Unit>;