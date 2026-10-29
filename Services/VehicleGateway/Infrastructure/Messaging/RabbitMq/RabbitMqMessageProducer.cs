using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using VehicleGateway.Application.Interfaces;
using VehicleGateway.Options;

namespace VehicleGateway.Infrastructure.Messaging.RabbitMq;

public class RabbitMqMessageProducer : IMessageProducer
{
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly ILogger<RabbitMqMessageProducer> _logger;
    private readonly RabbitMQOptions _settings;
    private readonly ResiliencePipeline _resiliencePipeline;

    public RabbitMqMessageProducer(
         IRabbitMqConnection rabbitMqConnection,
         ILogger<RabbitMqMessageProducer> logger,
         IOptions<RabbitMQOptions> options,
         ResiliencePipelineProvider<string> pipelineProvider)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _logger = logger;
        _settings = options.Value;
        _resiliencePipeline = pipelineProvider.GetPipeline("rabbitmq-publish");
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var queueName = _settings.QueueName;

        var messageId = Guid.NewGuid().ToString();

        try
        {
            // Tüm RabbitMQ işlem süreci (bağlantı alma, kanal açma, declare ve publish) Polly ile sarıldı
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                var connection = await _rabbitMqConnection.GetConnectionAsync(ct);

                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(
                        publisherConfirmationsEnabled: true,
                        publisherConfirmationTrackingEnabled: true
                    ), ct);

                var returned = false;
                channel.BasicReturnAsync += (sender, ea) =>
                {
                    returned = true;

                    _logger.LogError("Mesaj yönlendirilemedi. RoutingKey: {RoutingKey}, Sebep: {ReplyText}", ea.RoutingKey, ea.ReplyText);

                    return Task.CompletedTask;
                };

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: _settings.Durable,
                    exclusive: _settings.Exclusive,
                    autoDelete: _settings.AutoDelete,
                    arguments: _settings.Arguments,
                    cancellationToken: ct);

                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties
                    {
                        Persistent = true,
                        MessageId = messageId
                    },
                    body: body,
                    cancellationToken: ct);

                if (Volatile.Read(ref returned))
                {
                    throw new InvalidOperationException($"Mesaj {queueName} kuyruğuna yönlendirilemedi (mandatory return).");
                }

                _logger.LogInformation("Mesaj başarıyla {QueueName} kuyruğuna gönderildi {JsonMessage}.", queueName, jsonMessage);

            }, cancellationToken);
        }
        catch (Exception ex)
        {
            // Polly'nin tüm deneme hakları tükenip hata fırlattığı son nokta
            _logger.LogError(ex, "Mesaj kuyruğa gönderilirken hata oluştu ve tüm denemeler başarısız oldu. Kuyruk: {QueueName}", queueName);
            throw;
        }
    }
}