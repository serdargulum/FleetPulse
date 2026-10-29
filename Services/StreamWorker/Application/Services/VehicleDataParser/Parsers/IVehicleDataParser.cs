using System.Text.Json;
using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Services.VehicleDataParser.Parsers;

public interface IVehicleDataParser
{
    string VehicleType { get; }

    UnifiedTelemetryDto Generate(JsonElement data);
}