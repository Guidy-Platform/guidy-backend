using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Notifications.Events;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence;

namespace CoursePlatform.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IMessagePublisher _publisher;

    public NotificationService(
        AppDbContext context,
        IMessagePublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task SendAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? actionUrl = null,
        bool sendEmail = false,
        string? emailAddress = null,
        CancellationToken ct = default)
    {
        // store in DB in app 
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ActionUrl = actionUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notifications.AddAsync(notification, ct);
        await _context.SaveChangesAsync(ct);

        // if need to send email, publish event to RabbitMQ
        if (sendEmail && !string.IsNullOrEmpty(emailAddress))
        {
            await _publisher.PublishAsync(new SendNotificationEvent
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                SendEmail = true,
                EmailAddress = emailAddress
            }, "notification.send", ct);
        }
    }

    public async Task SendToMultipleAsync(
        IEnumerable<Guid> userIds,
        string title,
        string message,
        NotificationType type,
        string? actionUrl = null,
        CancellationToken ct = default)
    {
        var notifications = userIds.Select(uid => new Notification
        {
            UserId = uid,
            Title = title,
            Message = message,
            Type = type,
            ActionUrl = actionUrl,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _context.Notifications.AddRangeAsync(notifications, ct);
        await _context.SaveChangesAsync(ct);
    }
}