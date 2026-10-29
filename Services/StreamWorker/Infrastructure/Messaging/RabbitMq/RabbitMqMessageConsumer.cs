using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StreamWorker.Application.Interfaces;
using StreamWorker.Options;

namespace StreamWorker.Infrastructure.Messaging.RabbitMq;

public class RabbitMqMessageConsumer(
        IRabbitMqConnection rabbitMqConnection,
        ILogger<RabbitMqMessageConsumer> logger,
        IOptions<RabbitMQOptions> options) : IMessageConsumer
{
    public async Task ConsumeAsync(Func<string, Task> handler, CancellationToken cancellationToken = default)
    {
        var queueName = options.Value.QueueName;

        var channel = await CreateConfiguredChannelAsync(cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                await handler(message);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // To DLQ
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mesaj işlenirken hata oluştu.");

                await HandleFailureAsync(channel, ea);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation("RabbitMQ consumer başlatıldı. Queue: {QueueName}", queueName);
    }

    private async Task HandleFailureAsync(IChannel channel, BasicDeliverEventArgs ea)
    {
        var queueName = options.Value.QueueName;

        var retryCount = ea.BasicProperties.Headers?.TryGetValue("retry-count", out var value) == true
            ? Convert.ToInt32(value)
            : 0;

        if (retryCount >= 1)
        {
            await channel.BasicNackAsync(
                ea.DeliveryTag,
                multiple: false,
                requeue: false);

            return;
        }

        var properties = new BasicProperties
        {
            ContentType = ea.BasicProperties.ContentType,
            ContentEncoding = ea.BasicProperties.ContentEncoding,
            CorrelationId = ea.BasicProperties.CorrelationId,
            MessageId = ea.BasicProperties.MessageId,
            Type = ea.BasicProperties.Type,
            AppId = ea.BasicProperties.AppId,
            Headers = new Dictionary<string, object?>(ea.BasicProperties.Headers ?? new Dictionary<string, object?>())
            {
                ["retry-count"] = retryCount + 1
            }
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: true,
            basicProperties: properties,
            body: ea.Body);

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }

    private async Task<IChannel> CreateConfiguredChannelAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;

        var connection = await rabbitMqConnection.GetConnectionAsync(cancellationToken);

        var channel = await connection.CreateChannelAsync(null, cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var queueName = settings.QueueName;

        await channel.ExchangeDeclareAsync(
            exchange: "vehicle-data-dlx",
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: "vehicle-data-dead",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: "vehicle-data-dead",
            exchange: "vehicle-data-dlx",
            routingKey: "vehicle-data-dead",
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: settings.Durable,
            exclusive: settings.Exclusive,
            autoDelete: settings.AutoDelete,
            arguments: settings.Arguments,
            cancellationToken: cancellationToken);

        return channel;
    }
}