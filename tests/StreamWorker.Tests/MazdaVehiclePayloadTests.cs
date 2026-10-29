using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;

namespace StreamWorker.Tests;

public class MazdaVehiclePayloadTests
{
    [Fact]
    public void Generate_ValidPayload_ReturnsNormalizedTelemetry()
    {
        using var doc = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "position": { "lat": 41.0082, "lng": 28.9784, "kmh": 80 },
            "motor": { "tempC": 90, "fuelPct": 60.5 },
            "recordedAt": "2026-10-16T10:00:00",
            "alerts": { "errorCodes": { "P0300": "Engine Misfire" } }
        }
        """);

        var result = new MazdaVehiclePayload().Generate(doc.RootElement);

        Assert.Equal("34ABC123", result.Plate);
        Assert.Equal(41.0082m, result.Latitude);
        Assert.Equal(28.9784m, result.Longitude);
        Assert.Equal(80m, result.Speed);
        Assert.Equal(90m, result.EngineTemperature);
        Assert.Equal(60.5, result.FuelLevel);
        Assert.Equal(DateTime.Parse("2026-10-16T10:00:00Z").ToUniversalTime(), result.RecordedAt);
        Assert.Equal("Engine Misfire", result.ErrorList["P0300"]);
    }
}