using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using VehicleGateway.Infrastructure.Messaging.RabbitMq;
using VehicleGateway.Options;
using Xunit;

namespace VehicleGateway.Tests;

public class RabbitMqMessageProducerTests
{
    [Fact]
    public async Task PublishAsync_WhenSuccessful_PublishesMessage()
    {
        var connection = new Mock<IRabbitMqConnection>();
        var logger = Mock.Of<ILogger<RabbitMqMessageProducer>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions
        {
            QueueName = "test-queue",
            Durable = false,
            Exclusive = false,
            AutoDelete = false
        });

        var channel = new Mock<IChannel>();
        var rabbitConnection = new Mock<IConnection>();

        rabbitConnection
            .Setup(x => x.CreateChannelAsync(
                It.IsAny<CreateChannelOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel.Object);

        connection
            .Setup(x => x.GetConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rabbitConnection.Object);

        channel
            .Setup(x => x.QueueDeclareAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueDeclareOk("test-queue", 0, 0));

        channel
            .Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var pipelineProvider = new Mock<ResiliencePipelineProvider<string>>();
        pipelineProvider
            .Setup(x => x.GetPipeline("rabbitmq-publish"))
            .Returns(ResiliencePipeline.Empty);

        var producer = new RabbitMqMessageProducer(
            connection.Object,
            logger,
            options,
            pipelineProvider.Object);

        await producer.PublishAsync(new { Plate = "34ABC123" });

        channel.Verify(x => x.BasicPublishAsync(
            It.IsAny<string>(),
            "test-queue",
            true,
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenPublishFails_ThrowsException()
    {
        var connection = new Mock<IRabbitMqConnection>();
        var logger = Mock.Of<ILogger<RabbitMqMessageProducer>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions
        {
            QueueName = "test-queue",
            Durable = false,
            Exclusive = false,
            AutoDelete = false
        });

        var channel = new Mock<IChannel>();
        var rabbitConnection = new Mock<IConnection>();

        rabbitConnection
            .Setup(x => x.CreateChannelAsync(
                It.IsAny<CreateChannelOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel.Object);

        connection
            .Setup(x => x.GetConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rabbitConnection.Object);

        channel
            .Setup(x => x.QueueDeclareAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueDeclareOk("test-queue", 0, 0));

        channel
            .Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Publish failed"));

        var pipelineProvider = new Mock<ResiliencePipelineProvider<string>>();
        pipelineProvider
            .Setup(x => x.GetPipeline("rabbitmq-publish"))
            .Returns(ResiliencePipeline.Empty);

        var producer = new RabbitMqMessageProducer(
            connection.Object,
            logger,
            options,
            pipelineProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => producer.PublishAsync(new { Plate = "34ABC123" }));
    }
}