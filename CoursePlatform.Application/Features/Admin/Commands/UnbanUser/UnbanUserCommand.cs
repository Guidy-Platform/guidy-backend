using MediatR;

namespace CoursePlatform.Application.Features.Admin.Commands.UnbanUser;

public record UnbanUserCommand(Guid UserId) : IRequest<Unit>;