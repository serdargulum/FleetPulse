using System.Text.Json;
using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Services.VehicleDataParser.Parsers;

public class ToyotaVehiclePayload : BaseParser, IVehicleDataParser
{
    public string VehicleType => "toyota";

    public UnifiedTelemetryDto Generate(JsonElement data)
    {
        var errorList = data
            .GetProperty("dtcCodes")
            .Deserialize<Dictionary<string, string>>() ?? [];

        return new UnifiedTelemetryDto
        {
            Plate = data.GetProperty("plate").GetString(),
            Latitude = data.GetProperty("latitude").GetDecimal(),
            Longitude = data.GetProperty("longitude").GetDecimal(),
            Speed = data.GetProperty("speed").GetDecimal(),
            EngineTemperature = data.GetProperty("coolantTemp").GetDecimal(),
            FuelLevel = data.GetProperty("fuelLevelPercentage").GetDouble(),
            RecordedAt = data.GetProperty("timestamp").GetDateTime(),
            ErrorList = errorList
        };
    }
}