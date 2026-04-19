using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Notifications.Events;

public class SendNotificationEvent
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public bool SendEmail { get; set; }
    public string? EmailAddress { get; set; }
}