using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Notifications.DTOs;
using CoursePlatform.Application.Features.Notifications.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, GetMyNotificationsResult>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<GetMyNotificationsResult> Handle(
        GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // filter by userId and  by unread only
        var spec = new MyNotificationsSpec(userId, request.UnreadOnly);
        var notifications = await _uow.Repository<Notification>()
                                      .GetAllWithSpecAsync(spec, ct);

        // number of unread notifications always
        var unreadSpec = new MyNotificationsSpec(userId, unreadOnly: true);
        var unreadCount = await _uow.Repository<Notification>()
                                    .CountAsync(unreadSpec, ct);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            ActionUrl = n.ActionUrl,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt
        }).ToList();

        return new GetMyNotificationsResult(dtos, unreadCount);
    }
}