
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Application.Queries;
using AdminApi.Application.Queries.GetVehicle;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Queries;

public class GetVehicleQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();
    private readonly Mock<ITelemetryRecordRepository> _telemetryRepository = new();
    private readonly Mock<ITelemetryAlertRepository> _alertRepository = new();

    private GetVehicleQueryHandler CreateHandler() =>
        new(
            _vehicleRepository.Object,
            _telemetryRepository.Object,
            _alertRepository.Object);

    [Fact]
    public async Task Handle_WhenVehicleDoesNotExist_ReturnsNull()
    {
        var vehicleId = Guid.NewGuid();

        _vehicleRepository
            .Setup(x => x.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var result = await CreateHandler().Handle(
            new GetVehicleQuery(vehicleId),
            CancellationToken.None);

        Assert.Null(result);

        _telemetryRepository.Verify(
            x => x.GetLatestByVehicleIdAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _alertRepository.Verify(
            x => x.GetRecentByVehicleIdAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenVehicleExists_MapsVehicleAndRelatedData()
    {
        var vehicleId = Guid.NewGuid();

        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Plate = "34ABC123",
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            IsActive = true
        };

        var telemetryRecord = new TelemetryRecord
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            Latitude = 41.0082m,
            Longitude = 28.9784m,
            Speed = 60,
            EngineTemperature = 90,
            FuelLevel = 75,
            RecordedAt = DateTime.UtcNow
        };

        var alert = new TelemetryAlert
        {
            VehicleId = vehicleId,
            Description = "High engine temperature",
            AlertType = "Warning",
            Timestamp = DateTime.UtcNow
        };

        _vehicleRepository
            .Setup(x => x.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        _telemetryRepository
            .Setup(x => x.GetLatestByVehicleIdAsync(
                vehicleId, 40, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TelemetryRecord> { telemetryRecord });

        _alertRepository
            .Setup(x => x.GetRecentByVehicleIdAsync(
                vehicleId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TelemetryAlert> { alert });

        var result = await CreateHandler().Handle(
            new GetVehicleQuery(vehicleId),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(vehicleId, result.Id);
        Assert.Equal("34ABC123", result.Plate);
        Assert.Equal("Toyota", result.Brand);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(2024, result.Year);
        Assert.True(result.IsActive);

        var mappedTelemetry = Assert.Single(
            Assert.IsAssignableFrom<IEnumerable<TelemetryDto>>(result.LatestTelemetry));

        Assert.Equal(telemetryRecord.Id, mappedTelemetry.Id);
        Assert.Equal(vehicleId, mappedTelemetry.VehicleId);
        Assert.Equal(60, mappedTelemetry.Speed);
        Assert.Equal(90, mappedTelemetry.EngineTemperature);
        Assert.Equal(75, mappedTelemetry.FuelLevel);

        var mappedAlert = Assert.Single(
            Assert.IsAssignableFrom<IEnumerable<AlertDto>>(result.AlertList));

        Assert.Equal(vehicleId, mappedAlert.VehicleId);
        Assert.Equal("High engine temperature", mappedAlert.Message);
        Assert.Equal("Warning", mappedAlert.AlertType);
    }

    [Fact]
    public async Task Handle_WhenVehicleExists_RequestsExpectedRecordLimits()
    {
        var vehicleId = Guid.NewGuid();

        _vehicleRepository
            .Setup(x => x.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Vehicle { Id = vehicleId });

        _telemetryRepository
            .Setup(x => x.GetLatestByVehicleIdAsync(
                vehicleId, 40, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TelemetryRecord>());

        _alertRepository
            .Setup(x => x.GetRecentByVehicleIdAsync(
                vehicleId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TelemetryAlert>());

        await CreateHandler().Handle(
            new GetVehicleQuery(vehicleId),
            CancellationToken.None);

        _telemetryRepository.Verify(
            x => x.GetLatestByVehicleIdAsync(
                vehicleId, 40, It.IsAny<CancellationToken>()),
            Times.Once);

        _alertRepository.Verify(
            x => x.GetRecentByVehicleIdAsync(
                vehicleId, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
