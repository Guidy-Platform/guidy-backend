using FluentValidation;

namespace CoursePlatform.Application.Features.Admin.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator
    : AbstractValidator<ChangeUserRoleCommand>
{
    private static readonly string[] AllowedRoles = ["Student", "Instructor"];

    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.NewRole)
            .NotEmpty()
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage("Role must be 'Student' or 'Instructor'.");
    }
}