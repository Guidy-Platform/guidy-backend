using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Admin.Commands.BanUser;

public class BanUserCommandHandler
    : IRequestHandler<BanUserCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notifications;

    public BanUserCommandHandler(
        IUserRepository userRepo,
        INotificationService notifications)
    {
        _userRepo = userRepo;
        _notifications = notifications;
    }

    public async Task<Unit> Handle(
        BanUserCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        if (user.IsBanned)
            throw new BadRequestException("User is already banned.");

        user.IsBanned = true;
        user.BanReason = request.Reason;
        await _userRepo.UpdateAsync(user, ct);

        // Notify the user
        await _notifications.SendAsync(
            userId: user.Id,
            title: "Account Suspended",
            message: $"Your account has been suspended. Reason: {request.Reason}",
            type: NotificationType.SystemMessage,
            ct: ct);

        return Unit.Value;
    }
}