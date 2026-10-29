using RabbitMQ.Client;

namespace StreamWorker.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqConnection
{
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);
}