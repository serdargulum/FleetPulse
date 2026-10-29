using System.Text.Json;

namespace VehicleGateway.Application.Commands.StoreVehicleData;

public record StoreVehicleDataCommand(JsonElement Data);