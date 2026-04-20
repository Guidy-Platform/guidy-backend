using MediatR;

namespace CoursePlatform.Application.Features.Admin.Commands.BanUser;

public record BanUserCommand(Guid UserId, string Reason) : IRequest<Unit>;