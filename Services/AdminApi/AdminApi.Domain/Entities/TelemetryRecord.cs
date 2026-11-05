using NetTopologySuite.Geometries;

namespace AdminApi.Domain.Entities;

public class TelemetryRecord
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Point? Location { get; set; }
    public decimal Speed { get; set; }
    public decimal EngineTemperature { get; set; }
    public double FuelLevel { get; set; }
    public DateTime RecordedAt { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
}