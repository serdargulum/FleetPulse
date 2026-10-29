using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;
using Xunit;

public class BmwVehiclePayloadTests
{
    [Fact]
    public void Generate_ValidPayload_ReturnsNormalizedTelemetry()
    {
        const string json = """
        {
            "plate": "34ABC123",
            "location": {
                "latitude": 41.0082,
                "longitude": 28.9784,
                "speed": 80
            },
            "engine": {
                "coolantTemp": 90,
                "fuelLevelPercentage": 60
            },
            "timestamp": "2026-10-16T10:00:00",
            "diagnostics": {
                "dtcCodes": {
                    "P0300": "Engine Misfire"
                }
            }
        }
        """;

        using var document = JsonDocument.Parse(json);
        var parser = new BmwVehiclePayload();

        var result = parser.Generate(document.RootElement);

        Assert.Equal("34ABC123", result.Plate);
        Assert.Equal(41.0082m, result.Latitude);
        Assert.Equal(28.9784m, result.Longitude);
        Assert.Equal(80, result.Speed);
        Assert.Equal(90, result.EngineTemperature);
        Assert.Equal(60, result.FuelLevel);
        Assert.Equal("Engine Misfire", result.ErrorList["P0300"]);
    }

    [Fact]
    public void Generate_MissingLocation_ThrowsKeyNotFoundException()
    {
        using var document = JsonDocument.Parse("""{"plate":"34ABC123"}""");
        var parser = new BmwVehiclePayload();

        Assert.Throws<KeyNotFoundException>(() => parser.Generate(document.RootElement));
    }
}