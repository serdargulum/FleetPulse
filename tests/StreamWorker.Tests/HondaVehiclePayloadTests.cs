using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;

namespace StreamWorker.Tests;

public class HondaVehiclePayloadTests
{
    [Fact]
    public void Generate_ValidPayload_ReturnsNormalizedTelemetry()
    {
        using var doc = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "gps": { "lat": 41.0082, "lon": 28.9784, "velocity": 80 },
            "powertrain": { "temperature": 90, "fuel": 60.5 },
            "event_time": 1792144800,
            "faults": { "codes": { "P0300": "Engine Misfire" } }
        }
        """);

        var result = new HondaVehiclePayload().Generate(doc.RootElement);

        Assert.Equal("34ABC123", result.Plate);
        Assert.Equal(41.0082m, result.Latitude);
        Assert.Equal(28.9784m, result.Longitude);
        Assert.Equal(80m, result.Speed);
        Assert.Equal(90m, result.EngineTemperature);
        Assert.Equal(60.5, result.FuelLevel);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1792144800).UtcDateTime, result.RecordedAt);
        Assert.Equal("Engine Misfire", result.ErrorList["P0300"]);
    }
}