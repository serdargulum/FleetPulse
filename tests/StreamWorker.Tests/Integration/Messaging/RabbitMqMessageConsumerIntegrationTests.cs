using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using StreamWorker.Infrastructure.Messaging.RabbitMq;
using StreamWorker.Options;
using Testcontainers.RabbitMq;
using Xunit;

namespace StreamWorker.Tests.Integration.Messaging;

public class RabbitMqMessageConsumerIntegrationTests
{
    // Integration testleri için izole bir RabbitMQ konteyneri oluşturur.
    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder("rabbitmq:4-management").Build();

    // Publish edilen mesajın consumer tarafından handler'a iletilmesini doğrular.
    [Fact]
    public async Task ConsumeAsync_MessagePublished_HandlerReceivesMessage()
    {
        await _rabbitMq.StartAsync();

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.GetConnectionString())
        };

        await using var connection = await factory.CreateConnectionAsync();

        var queueName = $"test-queue-{Guid.NewGuid():N}";

        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions
        {
            QueueName = queueName,
            Durable = true,
            Exclusive = false,
            AutoDelete = true,
            Arguments = null
        });

        var logger = LoggerFactory.Create(builder => { }).CreateLogger<RabbitMqMessageConsumer>();

        var rabbitMqConnection = new TestRabbitMqConnection(connection);

        var consumer = new RabbitMqMessageConsumer(rabbitMqConnection, logger, options);

        var received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var cts = new CancellationTokenSource();

        await consumer.ConsumeAsync(
            message =>
            {
                received.TrySetResult(message);
                return Task.CompletedTask;
            },
            cts.Token);

        await using var publisherChannel = await connection.CreateChannelAsync();

        await publisherChannel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: Encoding.UTF8.GetBytes("integration-test"));

        var actual = await received.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal("integration-test", actual);

        cts.Cancel();
        await _rabbitMq.DisposeAsync();
    }

    // Mevcut RabbitMQ bağlantısını test consumer'ına sağlar.
    private sealed class TestRabbitMqConnection(IConnection connection) : IRabbitMqConnection
    {
        public Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default) => Task.FromResult(connection);
    }

    // Handler ilk denemede hata verdiğinde mesajın yeniden işlenmesini doğrular.
    [Fact]
    public async Task ConsumeAsync_HandlerFailsOnce_MessageIsRetried()
    {
        await _rabbitMq.StartAsync();

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.GetConnectionString())
        };

        await using var connection = await factory.CreateConnectionAsync();

        var queueName = $"test-queue-{Guid.NewGuid():N}";

        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions
        {
            QueueName = queueName,
            Durable = true,
            Exclusive = false,
            AutoDelete = true,
            Arguments = null
        });

        using var loggerFactory = LoggerFactory.Create(builder => { });
        var logger = loggerFactory.CreateLogger<RabbitMqMessageConsumer>();

        var rabbitMqConnection = new TestRabbitMqConnection(connection);

        var consumer = new RabbitMqMessageConsumer(rabbitMqConnection, logger, options);

        var attempts = 0;
        var processed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        await consumer.ConsumeAsync(
            _ =>
            {
                if (Interlocked.Increment(ref attempts) == 1)
                {
                    throw new InvalidOperationException("Test retry");
                }

                processed.TrySetResult(true);
                return Task.CompletedTask;
            });

        await using var publisherChannel = await connection.CreateChannelAsync();

        await publisherChannel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: Encoding.UTF8.GetBytes("retry-test"));

        await processed.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(2, attempts);
    }

    // Handler iki kez hata verdiğinde mesajın Dead Letter Queue'ya (DLQ) yönlendirildiğini doğrular.
    [Fact]
    public async Task ConsumeAsync_HandlerFailsTwice_MessageGoesToDlq()
    {
        await _rabbitMq.StartAsync();

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.GetConnectionString())
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var setupChannel = await connection.CreateChannelAsync();

        var queueName = $"test-queue-{Guid.NewGuid():N}";
        const string dlx = "vehicle-data-dlx";
        const string dlq = "vehicle-data-dead";

        // DLQ altyapısını oluşturur.
        await setupChannel.ExchangeDeclareAsync(dlx, ExchangeType.Direct, durable: true);
        await setupChannel.QueueDeclareAsync(dlq, durable: true, exclusive: false, autoDelete: false);
        await setupChannel.QueueBindAsync(dlq, dlx, dlq);

        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions
        {
            QueueName = queueName,
            Durable = true,
            Exclusive = false,
            AutoDelete = true,
            Arguments = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = dlx,
                ["x-dead-letter-routing-key"] = dlq
            }
        });

        using var loggerFactory = LoggerFactory.Create(builder => { });
        var logger = loggerFactory.CreateLogger<RabbitMqMessageConsumer>();

        var consumer = new RabbitMqMessageConsumer(
            new TestRabbitMqConnection(connection),
            logger,
            options);

        var attempts = 0;
        var failedTwice = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        await consumer.ConsumeAsync(_ =>
        {
            if (Interlocked.Increment(ref attempts) == 2)
                failedTwice.TrySetResult(true);

            throw new InvalidOperationException("Test failure");
        });

        await using var publisherChannel = await connection.CreateChannelAsync();

        await publisherChannel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: Encoding.UTF8.GetBytes("dlq-test"));

        await failedTwice.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // Mesajın DLQ'ya ulaştığını doğrular.
        BasicGetResult? result = null;

        var timeout = DateTime.UtcNow.AddSeconds(10);

        while (DateTime.UtcNow < timeout)
        {
            result = await setupChannel.BasicGetAsync(dlq, autoAck: true);

            if (result is not null)
                break;

            await Task.Delay(200);
        }

        Assert.NotNull(result);
        Assert.Equal("dlq-test", Encoding.UTF8.GetString(result.Body.ToArray()));
        Assert.Equal(2, attempts);
    }

}