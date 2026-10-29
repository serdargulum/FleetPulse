
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleDataSimulator.Application.Interfaces;
using VehicleDataSimulator.Application.Services;
using VehicleDataSimulator.Application.Services.Generators;
using VehicleDataSimulator.Worker;
using Xunit;

namespace VehicleDataSimulator.Tests;

public class VehicleDataWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStarted_ProcessesVehicleTick()
    {
        var processed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var mock = new Mock<VehicleDataGeneratorService>(
            Mock.Of<IVehicleApiClient>(),
            Array.Empty<ITelemetryPayload>(),
            Mock.Of<ILogger<VehicleDataGeneratorService>>());

        mock
.Setup(x => x.ProcessVehicleTickAsync(It.IsAny<CancellationToken>()))
.Returns(() =>
{
    processed.TrySetResult();
    return Task.CompletedTask;
});

        var services = new ServiceCollection();
        services.AddSingleton(mock.Object);

        var serviceProvider = services.BuildServiceProvider();

        var worker = new VehicleDataWorker(
            Mock.Of<ILogger<VehicleDataWorker>>(),
            serviceProvider);

        await worker.StartAsync(CancellationToken.None);

        try
        {
            await processed.Task.WaitAsync(TimeSpan.FromSeconds(2));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
            worker.Dispose();
            serviceProvider.Dispose();
        }

        Assert.True(processed.Task.IsCompletedSuccessfully);
        mock.Verify(
            x => x.ProcessVehicleTickAsync(It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);

    }
}
