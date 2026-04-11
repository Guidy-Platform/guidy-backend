using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Auth.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CoursePlatform.Infrastructure.Services.Consumers;

public class EmailConsumer : BackgroundService
{
    private readonly IConnection? _connection;
    private readonly IServiceProvider _services;
    private readonly ILogger<EmailConsumer> _logger;

    public EmailConsumer(
        IConnection? connection,
        IServiceProvider services,
        ILogger<EmailConsumer> logger)
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
                "RabbitMQ unavailable. EmailConsumer will not start.");
            return;
        }

        // channel واحد للـ consumer — بيفضل مفتوح
        var channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await SetupQueueAsync<UserRegisteredEvent>(
            channel, "user.registered",
            HandleUserRegisteredAsync, ct);

        await SetupQueueAsync<PasswordResetRequestedEvent>(
            channel, "password.reset.requested",
            HandlePasswordResetAsync, ct);

        _logger.LogInformation("EmailConsumer started and listening.");

        // يفضل شغال لحد ما الـ app يوقف
        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task SetupQueueAsync<T>(
        IChannel channel,
        string queueName,
        Func<T, IEmailService, Task> handler,
        CancellationToken ct)
    {
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        // prefetch = 1 — بياخد رسالة واحدة في الوقت
        await channel.BasicQosAsync(0, 1, false, ct);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<T>(json)!;

                await using var scope = _services.CreateAsyncScope();
                var emailService = scope.ServiceProvider
                                       .GetRequiredService<IEmailService>();

                await handler(msg, emailService);

                await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
                _logger.LogInformation(
                    "Processed message from '{Queue}'.", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing message from '{Queue}'.", queueName);

                // requeue: false → يروح dead letter queue
                await channel.BasicNackAsync(
                    ea.DeliveryTag, false, requeue: false, cancellationToken: ct);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);
    }

    private static async Task HandleUserRegisteredAsync(
        UserRegisteredEvent evt, IEmailService emailService)
    {
        await emailService.SendAsync(new EmailMessage(
            To: evt.Email,
            Subject: "Your verification code — CoursePlatform",
            Body: $"""
                      <div style="font-family:sans-serif;max-width:480px;margin:auto">
                        <h2>Welcome, {evt.FirstName}!</h2>
                        <p>Use the code below to verify your email:</p>
                        <div style="font-size:36px;font-weight:bold;
                                    letter-spacing:8px;text-align:center;
                                    padding:20px;background:#f5f5f5;
                                    border-radius:8px;margin:24px 0">
                          {evt.OtpCode}
                        </div>
                        <p>This code expires in <strong>10 minutes</strong>.</p>
                        <p style="color:#888;font-size:12px">
                          If you didn't create an account, ignore this email.
                        </p>
                      </div>
                      """
        ));
    }

    private static async Task HandlePasswordResetAsync(
        PasswordResetRequestedEvent evt, IEmailService emailService)
    {
        await emailService.SendAsync(new EmailMessage(
            To: evt.Email,
            Subject: "Reset your password — CoursePlatform",
            Body: $"""
                      <div style="font-family:sans-serif;max-width:480px;margin:auto">
                        <h2>Hi {evt.FirstName},</h2>
                        <p>Use the code below to reset your password:</p>
                        <div style="font-size:36px;font-weight:bold;
                                    letter-spacing:8px;text-align:center;
                                    padding:20px;background:#f5f5f5;
                                    border-radius:8px;margin:24px 0">
                          {evt.OtpCode}
                        </div>
                        <p>This code expires in <strong>10 minutes</strong>.</p>
                        <p style="color:#888;font-size:12px">
                          If you didn't request this, ignore this email.
                        </p>
                      </div>
                      """
        ));
    }
}