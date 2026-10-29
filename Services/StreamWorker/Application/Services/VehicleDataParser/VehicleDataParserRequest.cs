using System.Text.Json;

namespace StreamWorker.Application.Services.VehicleDataParser;

public record VehicleDataParserRequest(string VehicleType, JsonElement Json);