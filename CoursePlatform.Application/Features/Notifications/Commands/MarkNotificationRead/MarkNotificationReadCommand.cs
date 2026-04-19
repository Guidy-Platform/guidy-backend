using MediatR;

namespace CoursePlatform.Application.Features.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(int NotificationId) : IRequest<Unit>;