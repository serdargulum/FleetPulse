using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using VehicleGateway.Options;

namespace VehicleGateway.Infrastructure.Messaging.RabbitMq;

public class RabbitMqConnection : IRabbitMqConnection, IAsyncDisposable
{
    private volatile IConnection? connection;

    private readonly ConnectionFactory factory;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private volatile bool disposed;

    public RabbitMqConnection(IOptions<RabbitMQOptions> options)
    {
        var settings = options.Value;

        var host = settings.Host;
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new InvalidOperationException("RabbitMQ Host konfigürasyonu eksik.");
        }

        if (settings.Port <= 0 || settings.Port > 65535)
        {
            throw new InvalidOperationException($"Geçersiz RabbitMQ Port değeri: '{settings.Port}'");
        }

        var userName = settings.User;
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("RabbitMQ User konfigürasyonu eksik.");
        }

        var password = settings.Password;
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("RabbitMQ Password konfigürasyonu eksik.");
        }

        factory = new ConnectionFactory
        {
            HostName = host,
            Port = settings.Port,
            UserName = userName,
            Password = password,

            AutomaticRecoveryEnabled = true,                    // bağlantı kopunca yeniden bağlan
            TopologyRecoveryEnabled = true,                     // queue/exchange/binding/consumer'ları yeniden kur
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),  // deneme aralığı
            RequestedHeartbeat = TimeSpan.FromSeconds(30),      // kopmayı hızlı fark etmek için
            ClientProvidedName = "VehicleGateway"
        };
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var current = connection;
        if (current is not null)
        {
            // Recovery açık: IsOpen geçici false olabilir, dispose etmeyin.
            return current;
        }

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (connection is null)
            {
                connection = await factory.CreateConnectionAsync(cancellationToken);
            }

            return connection;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (connection != null)
        {
            try
            {
                await connection.DisposeAsync();
            }
            catch
            {
                // Suppress or log disposal exceptions if necessary
            }
        }

        semaphore.Dispose();
    }
}