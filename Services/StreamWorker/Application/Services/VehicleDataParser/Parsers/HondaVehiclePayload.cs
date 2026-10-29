using StreamWorker.Domain.Models;
using System.Text.Json;

namespace StreamWorker.Application.Services.VehicleDataParser.Parsers;

public class HondaVehiclePayload : BaseParser, IVehicleDataParser
{
    public string VehicleType => "honda";

    public UnifiedTelemetryDto Generate(JsonElement data)
    {
        var gps = data.GetProperty("gps");
        var powertrain = data.GetProperty("powertrain");

        var unixTime = data.GetProperty("event_time").GetInt64();

        var codes = data.GetProperty("faults").GetProperty("codes").Deserialize<Dictionary<string, string>>() ?? []; ;

        return new UnifiedTelemetryDto
        {
            Plate = data.GetProperty("plate").GetString(),

            Latitude = gps.GetProperty("lat").GetDecimal(),
            Longitude = gps.GetProperty("lon").GetDecimal(),
            Speed = gps.GetProperty("velocity").GetDecimal(),

            EngineTemperature = powertrain
                .GetProperty("temperature")
                .GetDecimal(),

            FuelLevel = powertrain
                .GetProperty("fuel")
                .GetDouble(),

            RecordedAt = DateTimeOffset
                .FromUnixTimeSeconds(unixTime)
                .UtcDateTime,

            ErrorList = codes
        };
    }
}