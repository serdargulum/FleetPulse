namespace AdminApi.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property (İlişkili alarmlar)
    public ICollection<TelemetryAlert> Alerts { get; set; } = new List<TelemetryAlert>();
    public ICollection<TelemetryRecord> TelemetryRecords { get; set; } = new List<TelemetryRecord>();
    public ICollection<BatchAnomaly> BatchAnomalies { get; set; } = new List<BatchAnomaly>();
}