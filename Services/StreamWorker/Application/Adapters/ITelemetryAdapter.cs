using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Adapters;

public interface ITelemetryAdapter
{
    bool CanHandle(string rawPayload);
    UnifiedTelemetryDto Adapt(string rawPayload);
}

// Örnek Standart Cihaz Adaptörü
public class StandardTelemetryAdapter : ITelemetryAdapter
{
    public bool CanHandle(string rawPayload)
    {
        // Gelen JSON formatına göre cihaz tipi tespiti yapılabilir
        return true;
    }

    public UnifiedTelemetryDto Adapt(string rawPayload)
    {
        // System.Text.Json ile yeni yapıya uygun deserialize işlemi
        var data = System.Text.Json.JsonSerializer.Deserialize<UnifiedTelemetryDto>(rawPayload);

        return new UnifiedTelemetryDto
        {
            Id = Guid.NewGuid(),
            VehicleId = data?.VehicleId ?? Guid.Empty,
            Latitude = data?.Latitude ?? 0,
            Longitude = data?.Longitude ?? 0,
            Speed = data?.Speed ?? 0,
            EngineTemperature = data?.EngineTemperature ?? 0,
            FuelLevel = data?.FuelLevel ?? 0,
            RecordedAt = data?.RecordedAt == default ? DateTime.UtcNow : data?.RecordedAt ?? DateTime.UtcNow
        };
    }
}

// Yardımcı ham model
internal class StandardDevicePayload
{
    public string Device { get; set; } = string.Empty;
    public double Temp { get; set; }
    public string State { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}