
using Microsoft.Extensions.Logging;
using Moq;
using VehicleDataSimulator.Application.Interfaces;
using VehicleDataSimulator.Application.Services;
using VehicleDataSimulator.Application.Services.Generators;
using Xunit;

namespace VehicleDataSimulator.Tests;

public class VehicleDataGeneratorServiceTests
{
    [Fact]
    public async Task ProcessVehicleTickAsync_WhenGeneratorExists_SendsGeneratedData()
    {
        var apiClient = new Mock<IVehicleApiClient>();
        apiClient
            .Setup(x => x.SendDataAsync(
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var generators = new ITelemetryPayload[]
        {
            CreateGenerator("BMW"),
            CreateGenerator("Mazda"),
            CreateGenerator("Toyota"),
            CreateGenerator("Honda")
        };

        var logger = Mock.Of<ILogger<VehicleDataGeneratorService>>();

        var sut = new VehicleDataGeneratorService(
            apiClient.Object,
            generators,
            logger);

        await sut.ProcessVehicleTickAsync(CancellationToken.None);

        apiClient.Verify(
            x => x.SendDataAsync(
                It.IsAny<object>(),
                It.IsAny<string>(),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task ProcessVehicleTickAsync_WhenApiReturnsFalse_CompletesWithoutThrowing()
    {
        var apiClient = new Mock<IVehicleApiClient>();
        apiClient
            .Setup(x => x.SendDataAsync(
                It.IsAny<object>(),
                 It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var generators = new ITelemetryPayload[]
        {
            CreateGenerator("BMW"),
            CreateGenerator("Mazda"),
            CreateGenerator("Toyota"),
            CreateGenerator("Honda")
        };

        var logger = Mock.Of<ILogger<VehicleDataGeneratorService>>();

        var sut = new VehicleDataGeneratorService(
            apiClient.Object,
            generators,
            logger);

        await sut.ProcessVehicleTickAsync(CancellationToken.None);
    }

    private static ITelemetryPayload CreateGenerator(string vehicleType)
    {
        var generator = new Mock<ITelemetryPayload>();
        generator.SetupGet(x => x.VehicleType).Returns(vehicleType);
        generator.Setup(x => x.Generate(It.IsAny<string>()))
            .Returns(new { Test = true });

        return generator.Object;
    }
}
