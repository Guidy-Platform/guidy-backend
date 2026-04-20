using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Commands.UnbanUser;

public class UnbanUserCommandHandler
    : IRequestHandler<UnbanUserCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notifications;

    public UnbanUserCommandHandler(
        IUserRepository userRepo,
        INotificationService notifications)
    {
        _userRepo = userRepo;
        _notifications = notifications;
    }

    public async Task<Unit> Handle(
        UnbanUserCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        if (!user.IsBanned)
            throw new BadRequestException("User is not banned.");

        user.IsBanned = false;
        user.BanReason = null;
        await _userRepo.UpdateAsync(user, ct);

        await _notifications.SendAsync(
            userId: user.Id,
            title: "Account Restored",
            message: "Your account has been restored. Welcome back!",
            type: NotificationType.SystemMessage,
            ct: ct);

        return Unit.Value;
    }
}