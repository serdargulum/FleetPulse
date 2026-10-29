using AdminApi.Application.Contracts.Repositories;
using Mediator;

namespace AdminApi.Application.Queries.GetVehicle;

public sealed class GetVehicleQueryHandler(
    IVehicleRepository vehicleRepository,
    ITelemetryRecordRepository telemetryRecordRepository,
    ITelemetryAlertRepository alertRepository)
    : IRequestHandler<GetVehicleQuery, VehicleDto?>
{
    public async ValueTask<VehicleDto?> Handle(
        GetVehicleQuery query,
        CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(
            query.Id, cancellationToken);

        if (vehicle is null)
            return null;

        var telemetryRecords =
            await telemetryRecordRepository.GetLatestByVehicleIdAsync(
                vehicle.Id, 40, cancellationToken);

        var alerts = await alertRepository.GetRecentByVehicleIdAsync(
            vehicle.Id, 10, cancellationToken);

        var telemetry = telemetryRecords.Select(x => new TelemetryDto
        {
            Id = x.Id,
            VehicleId = x.VehicleId,
            Latitude = (decimal)(x.Location?.Y ?? 0),
            Longitude = (decimal)(x.Location?.X ?? 0),
            Speed = x.Speed,
            EngineTemperature = x.EngineTemperature,
            FuelLevel = x.FuelLevel,
            RecordedAt = x.RecordedAt
        }).ToList();

        var alertList = alerts.Select(x => new AlertDto
        {
            VehicleId = x.VehicleId,
            Message = x.Description,
            AlertType = x.AlertType,
            CreatedAt = x.Timestamp
        }).ToList();

        return new VehicleDto
        {
            Id = vehicle.Id,
            Plate = vehicle.Plate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            IsActive = vehicle.IsActive,
            LatestTelemetry = telemetry,
            AlertList = alertList
        };
    }
}