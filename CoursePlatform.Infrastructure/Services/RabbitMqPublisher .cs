using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CoursePlatform.Infrastructure.Services;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection? _connection;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(
        IConnection? connection,
        ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        T message, string queueName, CancellationToken ct = default)
        where T : class
    {
        if (_connection is null || _connection.IsOpen == false)
        {
            _logger.LogWarning(
                "RabbitMQ unavailable. Skipping publish to '{Queue}'.", queueName);
            return;
        }

        try
        {
            // channel جديد لكل publish — بيتقفل بعد الإرسال فوراً
            await using var channel = await _connection.CreateChannelAsync(
                cancellationToken: ct);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: ct);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var props = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);

            _logger.LogInformation(
                "Published message to queue '{Queue}'.", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to publish to queue '{Queue}'.", queueName);
        }
    }
}