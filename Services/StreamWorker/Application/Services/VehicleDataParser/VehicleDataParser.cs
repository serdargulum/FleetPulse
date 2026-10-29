using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;
using StreamWorker.Domain.Models;
using StreamWorker.Application.Services.VehicleDataParser.Validators;

namespace StreamWorker.Application.Services.VehicleDataParser;

public class VehicleDataParser(IEnumerable<IVehicleDataParser> parsers, IEnumerable<IVehicleDataParserValidator> validators)
{
    public UnifiedTelemetryDto Generate(string vehicleType, JsonElement json)
    {
        var parser = parsers.FirstOrDefault(x =>
            x.VehicleType.Equals(vehicleType, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Bu marka için parser bulunamadı: {vehicleType}");

        var validator = validators.FirstOrDefault(x => x.VehicleType.Equals(vehicleType, StringComparison.OrdinalIgnoreCase))
                   ?? throw new InvalidOperationException($"Bu marka için validator bulunamadı: {vehicleType}");

        validator.ValidatePayload(json);

        return parser.Generate(json);
    }
}