// Infrastructure/Services/Consumers/NotificationConsumer.cs
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Notifications.Events;
using CoursePlatform.Application.Features.Orders.Events;
using CoursePlatform.Application.Features.Auth.Events;
using CoursePlatform.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CoursePlatform.Infrastructure.Services.Consumers;

public class NotificationConsumer : BackgroundService
{
    private readonly IConnection? _connection;
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationConsumer> _logger;
    private IChannel? _channel;

    public NotificationConsumer(
        IConnection? connection,
        IServiceProvider services,
        ILogger<NotificationConsumer> logger)
    {
        _connection = connection;
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (_connection is null)
        {
            _logger.LogWarning(
                "RabbitMQ unavailable. NotificationConsumer will not start.");
            return;
        }

        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        // استمع على الـ queues المختلفة
        await ConsumeAsync<OrderCompletedEvent>(
            "order.completed", HandleOrderCompletedAsync, ct);

        await ConsumeAsync<UserRegisteredEvent>(
             "user.registered",          
             HandleUserRegisteredAsync, ct);

        await ConsumeAsync<SendNotificationEvent>(
            "notification.send",
            HandleSendNotificationAsync, ct);

        _logger.LogInformation("NotificationConsumer started.");
        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task ConsumeAsync<T>(
        string queueName,
        Func<T, IServiceScope, Task> handler,
        CancellationToken ct)
    {
        await _channel!.QueueDeclareAsync(
            queue: queueName, durable: true,
            exclusive: false, autoDelete: false,
            cancellationToken: ct);

        await _channel.BasicQosAsync(0, 1, false, ct);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<T>(json)!;

                await using var scope = _services.CreateAsyncScope();
                await handler(msg, scope);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in NotificationConsumer queue '{Queue}'",
                    queueName);
                await _channel.BasicNackAsync(
                    ea.DeliveryTag, false, requeue: false,
                    cancellationToken: ct);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName, autoAck: false,
            consumer: consumer, cancellationToken: ct);
    }

    // ─── Handlers ────────────────────────────────────────────────

    private static async Task HandleOrderCompletedAsync(
        OrderCompletedEvent evt, IServiceScope scope)
    {
        var notificationService = scope.ServiceProvider
            .GetRequiredService<INotificationService>();

        var courseList = string.Join(", ", evt.CourseTitles);

        // In-app notification للـ Student
        await notificationService.SendAsync(
            userId: evt.StudentId,
            title: "Enrollment Confirmed!",
            message: $"You are now enrolled in: {courseList}",
            type: NotificationType.OrderCompleted,
            actionUrl: "/my-courses");

        // Email للـ Student (لو في email)
        if (!string.IsNullOrEmpty(evt.StudentEmail))
        {
            var emailService = scope.ServiceProvider
                .GetRequiredService<IEmailService>();

            await emailService.SendAsync(new EmailMessage(
                To: evt.StudentEmail,
                Subject: "Enrollment Confirmed — CoursePlatform",
                Body: BuildOrderConfirmationEmail(
                    evt.StudentName, courseList, evt.FinalPrice)
            ));
        }
    }

    private static async Task HandleUserRegisteredAsync(
        UserRegisteredEvent evt, IServiceScope scope)
    {
        var notificationService = scope.ServiceProvider
            .GetRequiredService<INotificationService>();

        await notificationService.SendAsync(
            userId: evt.UserId,
            title: $"Welcome to CoursePlatform, {evt.FirstName}!",
            message: "Start exploring courses and begin your learning journey.",
            type: NotificationType.WelcomeMessage,
            actionUrl: "/courses");
    }

    private static async Task HandleSendNotificationAsync(
        SendNotificationEvent evt, IServiceScope scope)
    {
        if (!evt.SendEmail || string.IsNullOrEmpty(evt.EmailAddress))
            return;

        var emailService = scope.ServiceProvider
            .GetRequiredService<IEmailService>();

        await emailService.SendAsync(new EmailMessage(
            To: evt.EmailAddress,
            Subject: evt.Title,
            Body: BuildGenericEmail(evt.Title, evt.Message, evt.ActionUrl)
        ));
    }

    // ─── Email Templates ─────────────────────────────────────────
    private static string BuildOrderConfirmationEmail(
        string studentName, string courses, decimal amount)
    {
        return $"""
    <div style="font-family:sans-serif;max-width:600px;margin:auto">
      <h2>Hi {studentName}!</h2>
      <p>Your payment was successful. You are now enrolled in:</p>
      <div style="background:#f5f5f5;padding:16px;border-radius:8px;margin:16px 0">
        <strong>{courses}</strong>
      </div>
      <p>Amount paid: <strong>${amount:F2}</strong></p>
      <a href="/my-courses"
         style="background:#7C3AED;color:white;padding:12px 24px;
                border-radius:6px;text-decoration:none;display:inline-block">
        Go to My Courses
      </a>
      <p style="color:#888;font-size:12px;margin-top:24px">
        CoursePlatform Team
      </p>
    </div>
    """;
    }

    private static string BuildGenericEmail(
     string title, string message, string? actionUrl)
    {
        var button = "";

        if (!string.IsNullOrEmpty(actionUrl))
        {
            button = $"""
        <a href="{actionUrl}"
           style="background:#7C3AED;color:white;padding:12px 24px;
                  border-radius:6px;text-decoration:none;display:inline-block">
          View Details
        </a>
        """;
        }

        return $"""
    <div style="font-family:sans-serif;max-width:600px;margin:auto">
      <h2>{title}</h2>
      <p>{message}</p>
      {button}
    </div>
    """;
    }
}