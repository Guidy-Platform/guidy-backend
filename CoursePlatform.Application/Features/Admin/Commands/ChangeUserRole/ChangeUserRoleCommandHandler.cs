using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Admin.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler
    : IRequestHandler<ChangeUserRoleCommand, Unit>
{
    private readonly IUserRepository _userRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationService _notifications;

    public ChangeUserRoleCommandHandler(
        IUserRepository userRepo,
        UserManager<AppUser> userManager,
        INotificationService notifications)
    {
        _userRepo = userRepo;
        _userManager = userManager;
        _notifications = notifications;
    }

    public async Task<Unit> Handle(
        ChangeUserRoleCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var currentRoles = await _userManager.GetRolesAsync(user);

        // مش ممكن تغير دور الـ Admin
        if (currentRoles.Contains("Admin"))
            throw new ForbiddenException(
                "Cannot change the role of an Admin.");

        // شيل كل الـ roles الحالية
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // أضيف الـ role الجديدة
        await _userManager.AddToRoleAsync(user, request.NewRole);

        await _notifications.SendAsync(
            userId: user.Id,
            title: "Role Updated",
            message: $"Your account role has been changed to {request.NewRole}.",
            type: NotificationType.SystemMessage,
            ct: ct);

        return Unit.Value;
    }
}