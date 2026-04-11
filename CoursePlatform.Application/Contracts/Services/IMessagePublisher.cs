// Application/Contracts/Services/IMessagePublisher.cs
namespace CoursePlatform.Application.Contracts.Services;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default)
        where T : class;
}