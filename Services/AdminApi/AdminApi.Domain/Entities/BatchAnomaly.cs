namespace AdminApi.Domain.Entities;

public class BatchAnomaly
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string AnomalyType { get; set; } = string.Empty; // Örn: "UnauthorizedNightUsage", "FuelDrainAnomaly"
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public DateTime AnalysisDate { get; set; }              // Hangi günün verisi tarandı

    public Vehicle Vehicle { get; set; } = null!;
}