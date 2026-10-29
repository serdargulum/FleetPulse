using RabbitMQ.Client;

namespace VehicleGateway.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqConnection
{
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}