using System.Globalization;
using System.Text.Json;
using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Services.VehicleDataParser.Parsers;

public class MazdaVehiclePayload : BaseParser, IVehicleDataParser
{
    public string VehicleType => "mazda";

    public UnifiedTelemetryDto Generate(JsonElement data)
    {
        var position = data.GetProperty("position");
        var motor = data.GetProperty("motor");

        var recordedAt = data.GetProperty("recordedAt").GetString()!;

        var errorList = data
            .GetProperty("alerts")
            .GetProperty("errorCodes")
            .Deserialize<Dictionary<string, string>>() ?? [];

        return new UnifiedTelemetryDto
        {
            Plate = data.GetProperty("plate").GetString(),
            Latitude = position.GetProperty("lat").GetDecimal(),
            Longitude = position.GetProperty("lng").GetDecimal(),
            Speed = position.GetProperty("kmh").GetDecimal(),
            EngineTemperature = motor.GetProperty("tempC").GetDecimal(),
            FuelLevel = motor.GetProperty("fuelPct").GetDouble(),
            RecordedAt = DateTime.Parse(recordedAt, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
            ErrorList = errorList
        };
    }
}