using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Notifications.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsReadCommand, int>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(
        MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        //get all  unread notifications
        var spec = new MyNotificationsSpec(userId, unreadOnly: true);
        var notifications = await _uow.Repository<Notification>()
                                      .GetAllWithSpecAsync(spec, ct);

        if (!notifications.Any()) return 0;

        var now = DateTime.UtcNow;
        foreach (var n in notifications)
        {
            n.IsRead = true;
            n.ReadAt = now;
            _uow.Repository<Notification>().Update(n);
        }

        await _uow.CompleteAsync(ct);
        return notifications.Count;
    }
}