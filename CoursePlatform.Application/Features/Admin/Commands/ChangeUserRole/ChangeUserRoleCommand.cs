using MediatR;

namespace CoursePlatform.Application.Features.Admin.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(
    Guid UserId,
    string NewRole   // "Student" | "Instructor"
) : IRequest<Unit>;