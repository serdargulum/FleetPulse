using AdminApi.Application.Contracts.Repositories;
using Mediator;

namespace AdminApi.Application.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler(
    IVehicleRepository vehicleRepository,
    ITelemetryAlertRepository alertRepository,
    ITelemetryRecordRepository telemetryRepository)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public async ValueTask<DashboardDto> Handle(
        GetDashboardQuery query,
        CancellationToken cancellationToken)
    {
        var vehicles = await vehicleRepository.GetAllAsync(cancellationToken);
        var vehiclePositions = await telemetryRepository.GetAllVehiclePositionsAsync(cancellationToken);
        var alerts = await alertRepository.GetRecentAsync(10, cancellationToken);

        return new DashboardDto
        {
            Summary = new DashboardSummaryDto
            {
                TotalVehicles = vehicles.Count,
                ActiveVehicles = vehicles.Count(x => x.IsActive == true),
                ActiveAlerts = alerts.Count
            },

            Alerts = [.. alerts.Select(x => new DashboardAlertDto
            {
                VehicleId = x.VehicleId,
                Plate = x.Vehicle.Plate,
                Message = x.Description,
                AlertType = x.AlertType,
                CreatedAt = x.Timestamp
            })],

            Vehicles = [.. vehicles.Select(x =>
            {
                var telemetry = x.TelemetryRecords
                    .OrderByDescending(t => t.RecordedAt)
                    .FirstOrDefault();

                return new DashboardVehicleDto
                {
                    Id = x.Id,
                    Plate = x.Plate,
                    Brand = x.Brand,
                    Model = x.Model,
                    IsActive = x.IsActive,
                    Speed = telemetry?.Speed ?? 0,
                    EngineTemperature = telemetry?.EngineTemperature ?? 0,
                    FuelLevel = (decimal)(telemetry?.FuelLevel ?? 0),
                    LastUpdatedAt = telemetry?.RecordedAt ?? DateTime.MinValue
                };
            })],

            Positions = [.. vehiclePositions.Select(p => new VehiclePositionDto
            {
                Plate = p.Vehicle.Plate,
                Latitude = (decimal)(p.Location?.Y ?? 0),
                Longitude = (decimal)(p.Location?.X ?? 0),
            })]
        };
    }
}