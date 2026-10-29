
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Application.Queries.GetDashboard;
using AdminApi.Domain.Entities;
using Moq;
using Xunit;

namespace AdminApi.Tests.Queries;

public class GetDashboardQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();
    private readonly Mock<ITelemetryAlertRepository> _alertRepository = new();
    private readonly Mock<ITelemetryRecordRepository> _telemetryRepository = new();

    private GetDashboardQueryHandler CreateHandler() =>
        new(
            _vehicleRepository.Object,
            _alertRepository.Object,
            _telemetryRepository.Object);

    [Fact]
    public async Task Handle_CalculatesDashboardSummary()
    {
        var vehicles = new List<Vehicle>
        {
            new() { Id = Guid.NewGuid(), IsActive = true },
            new() { Id = Guid.NewGuid(), IsActive = true },
            new() { Id = Guid.NewGuid(), IsActive = false }
        };

        var alerts = new List<TelemetryAlert>
        {
            new() { Vehicle = new Vehicle { Plate = "34ABC123" } },
            new() { Vehicle = new Vehicle { Plate = "34XYZ789" } }
        };

        SetupRepositories(vehicles, alerts, []);

        var result = await CreateHandler().Handle(
            new GetDashboardQuery(),
            CancellationToken.None);

        Assert.Equal(3, result.Summary.TotalVehicles);
        Assert.Equal(2, result.Summary.ActiveVehicles);
        Assert.Equal(2, result.Summary.ActiveAlerts);
    }

    [Fact]
    public async Task Handle_UsesLatestTelemetryForVehicle()
    {
        var vehicleId = Guid.NewGuid();
        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Plate = "34ABC123",
            Brand = "Toyota",
            Model = "Corolla",
            IsActive = true,
            TelemetryRecords =
            [
                new TelemetryRecord
                {
                    VehicleId = vehicleId,
                    Speed = 30m,
                    EngineTemperature = 80m,
                    FuelLevel = 50,
                    RecordedAt = new DateTime(2026, 1, 1)
                },
                new TelemetryRecord
                {
                    VehicleId = vehicleId,
                    Speed = 70m,
                    EngineTemperature = 95m,
                    FuelLevel = 25,
                    RecordedAt = new DateTime(2026, 1, 2)
                }
            ]
        };

        SetupRepositories([vehicle], [], []);

        var result = await CreateHandler().Handle(
            new GetDashboardQuery(),
            CancellationToken.None);

        var mappedVehicle = Assert.Single(result.Vehicles);
        Assert.Equal(70m, mappedVehicle.Speed);
        Assert.Equal(95m, mappedVehicle.EngineTemperature);
        Assert.Equal(25m, mappedVehicle.FuelLevel);
        Assert.Equal(new DateTime(2026, 1, 2), mappedVehicle.LastUpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenVehicleHasNoTelemetry_UsesDefaultValues()
    {
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Plate = "34ABC123",
            TelemetryRecords = []
        };

        SetupRepositories([vehicle], [], []);

        var result = await CreateHandler().Handle(
            new GetDashboardQuery(),
            CancellationToken.None);

        var mappedVehicle = Assert.Single(result.Vehicles);
        Assert.Equal(0m, mappedVehicle.Speed);
        Assert.Equal(0m, mappedVehicle.EngineTemperature);
        Assert.Equal(0m, mappedVehicle.FuelLevel);
        Assert.Equal(DateTime.MinValue, mappedVehicle.LastUpdatedAt);
    }

    [Fact]
    public async Task Handle_MapsAlertsAndVehiclePositions()
    {
        var vehicleId = Guid.NewGuid();
        var alert = new TelemetryAlert
        {
            VehicleId = vehicleId,
            Vehicle = new Vehicle { Plate = "34ABC123" },
            Description = "High engine temperature",
            AlertType = "Warning",
            Timestamp = new DateTime(2026, 1, 2)
        };

        var position = new TelemetryRecord
        {
            Vehicle = new Vehicle { Plate = "34XYZ789" },
            Latitude = 41.0082m,
            Longitude = 28.9784m
        };

        SetupRepositories([], [alert], [position]);

        var result = await CreateHandler().Handle(
            new GetDashboardQuery(),
            CancellationToken.None);

        var mappedAlert = Assert.Single(result.Alerts);
        Assert.Equal(vehicleId, mappedAlert.VehicleId);
        Assert.Equal("34ABC123", mappedAlert.Plate);
        Assert.Equal("High engine temperature", mappedAlert.Message);
        Assert.Equal("Warning", mappedAlert.AlertType);

        var mappedPosition = Assert.Single(result.Positions);
        Assert.Equal("34XYZ789", mappedPosition.Plate);
        Assert.Equal(41.0082m, mappedPosition.Latitude);
        Assert.Equal(28.9784m, mappedPosition.Longitude);
    }

    private void SetupRepositories(
        List<Vehicle> vehicles,
        List<TelemetryAlert> alerts,
        List<TelemetryRecord> positions)
    {
        _vehicleRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        _alertRepository
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        _telemetryRepository
            .Setup(x => x.GetAllVehiclePositionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);
    }
}
