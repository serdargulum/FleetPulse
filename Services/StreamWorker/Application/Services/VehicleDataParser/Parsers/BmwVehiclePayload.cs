using System.Text.Json;
using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Services.VehicleDataParser.Parsers;

public class BmwVehiclePayload : BaseParser, IVehicleDataParser
{
    public string VehicleType => "bmw";

    public UnifiedTelemetryDto Generate(JsonElement data)
    {
        var location = data.GetProperty("location");
        var engine = data.GetProperty("engine");
        var dtcCodes = data.GetProperty("diagnostics").GetProperty("dtcCodes").Deserialize<Dictionary<string, string>>() ?? [];

        return new UnifiedTelemetryDto
        {
            Plate = data.GetProperty("plate").GetString(),

            Latitude = location.GetProperty("latitude").GetDecimal(),
            Longitude = location.GetProperty("longitude").GetDecimal(),
            Speed = location.GetProperty("speed").GetInt32(),
            EngineTemperature = engine.GetProperty("coolantTemp").GetInt32(),
            FuelLevel = engine.GetProperty("fuelLevelPercentage").GetInt32(),
            RecordedAt = data.GetProperty("timestamp").GetDateTime(),
            ErrorList = dtcCodes
        };
    }

}