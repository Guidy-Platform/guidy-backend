using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Contracts.Services;

public interface INotificationService
{
    Task SendAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? actionUrl = null,
        bool sendEmail = false,
        string? emailAddress = null,
        CancellationToken ct = default);

    Task SendToMultipleAsync(
        IEnumerable<Guid> userIds,
        string title,
        string message,
        NotificationType type,
        string? actionUrl = null,
        CancellationToken ct = default);
}