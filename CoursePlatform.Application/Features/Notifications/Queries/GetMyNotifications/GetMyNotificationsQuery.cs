using CoursePlatform.Application.Features.Notifications.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(
    bool? UnreadOnly = null
) : IRequest<GetMyNotificationsResult>;

public record GetMyNotificationsResult(
    IReadOnlyList<NotificationDto> Notifications,
    int UnreadCount
);