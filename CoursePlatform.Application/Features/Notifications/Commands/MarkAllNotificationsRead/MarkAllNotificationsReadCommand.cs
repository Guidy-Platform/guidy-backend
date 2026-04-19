using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand : IRequest<int>;
// returns the number of notifications marked as read