using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;

namespace StreamWorker.Tests;

public class ToyotaVehiclePayloadTests
{
    [Fact]
    public void Generate_ValidPayload_ReturnsNormalizedTelemetry()
    {
        using var doc = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "latitude": 41.0082,
            "longitude": 28.9784,
            "speed": 80,
            "coolantTemp": 90,
            "fuelLevelPercentage": 60.5,
            "timestamp": "2026-10-16T10:00:00Z",
            "dtcCodes": { "P0300": "Engine Misfire" }
        }
        """);

        var result = new ToyotaVehiclePayload().Generate(doc.RootElement);

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