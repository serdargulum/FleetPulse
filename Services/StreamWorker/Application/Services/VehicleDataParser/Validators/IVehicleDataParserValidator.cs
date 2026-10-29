using System.Text.Json;

namespace StreamWorker.Application.Services.VehicleDataParser.Validators;

public interface IVehicleDataParserValidator
{
    string VehicleType { get; }
    void ValidatePayload(JsonElement json);
}