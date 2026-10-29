
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using VehicleGateway.Application.Commands.StoreVehicleData;
using VehicleGateway.Application.Interfaces;
using Xunit;

namespace VehicleGateway.Tests;

public class StoreVehicleDataHandlerTests
{
    [Fact]
    public async Task Handle_ValidData_PublishesMessage()
    {
        var producer = new FakeMessageProducer();
        var handler = new StoreVehicleDataHandler(producer);
        using var document = JsonDocument.Parse("""{"Plate":"34ABC123"}""");

        await handler.Handle(new StoreVehicleDataCommand(document.RootElement), CancellationToken.None);

        Assert.NotNull(producer.PublishedMessage);
        Assert.Contains("ReceivedAt", producer.PublishedMessage!);
    }

    [Fact]
    public async Task Handle_InvalidData_ThrowsValidationException()
    {
        var producer = new FakeMessageProducer();
        var handler = new StoreVehicleDataHandler(producer);
        using var document = JsonDocument.Parse("{}");

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new StoreVehicleDataCommand(document.RootElement), CancellationToken.None));

        Assert.Null(producer.PublishedMessage);
    }

    [Fact]
    public async Task Handle_ValidData_PassesCancellationTokenToProducer()
    {
        var producer = new FakeMessageProducer();
        var handler = new StoreVehicleDataHandler(producer);
        using var document = JsonDocument.Parse("""{"Plate":"34ABC123"}""");
        using var cts = new CancellationTokenSource();

        await handler.Handle(
            new StoreVehicleDataCommand(document.RootElement),
            cts.Token);

        Assert.Equal(cts.Token, producer.ReceivedToken);
    }

    private sealed class FakeMessageProducer : IMessageProducer
    {
        public string? PublishedMessage { get; private set; }

        public CancellationToken? ReceivedToken { get; private set; }

        public Task PublishAsync<T>(
            T message,
            CancellationToken cancellationToken = default)
        {
            PublishedMessage = message?.ToString();
            ReceivedToken = cancellationToken;
            return Task.CompletedTask;
        }
    }
}
