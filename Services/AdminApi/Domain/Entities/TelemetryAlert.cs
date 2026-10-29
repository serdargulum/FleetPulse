namespace AdminApi.Domain.Entities;

public class TelemetryAlert
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string AlertType { get; set; } = string.Empty; // Örn: "HighEngineTemperature", "FaultCode"
    public string Description { get; set; } = string.Empty;
    public decimal TriggerValue { get; set; }             // Tetiklenen değer (Örn: 115.5 derece)
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Foreign Key ilişki için
    public Vehicle Vehicle { get; set; } = null!;
}