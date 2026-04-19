using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Notifications.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        MarkNotificationReadCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new NotificationByIdSpec(
            request.NotificationId, userId);
        var notification = await _uow.Repository<Notification>()
                                     .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException(
                "Notification", request.NotificationId);

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _uow.Repository<Notification>().Update(notification);
            await _uow.CompleteAsync(ct);
        }

        return Unit.Value;
    }
}