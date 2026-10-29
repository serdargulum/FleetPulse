namespace StreamWorker.Domain.Models;

public class UnifiedTelemetryDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Plate { get; init; }
    public Guid VehicleId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public decimal Speed { get; init; }
    public decimal EngineTemperature { get; init; }
    public double FuelLevel { get; init; }
    public DateTime RecordedAt { get; init; } = DateTime.UtcNow;

    public Dictionary<string, string> ErrorList { get; init; } = [];
}