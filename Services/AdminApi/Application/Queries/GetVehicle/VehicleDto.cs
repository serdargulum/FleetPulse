namespace AdminApi.Application.Queries;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? Year { get; set; }
    public bool IsActive { get; set; }

    public List<TelemetryDto>? LatestTelemetry { get; set; }
    public List<AlertDto>? AlertList { get; set; }

}

public class AlertDto
{
    public Guid VehicleId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TelemetryDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Speed { get; set; }
    public decimal EngineTemperature { get; set; }
    public double FuelLevel { get; set; }
    public DateTime RecordedAt { get; set; }
}