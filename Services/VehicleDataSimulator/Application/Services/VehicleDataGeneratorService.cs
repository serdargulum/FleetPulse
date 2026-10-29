using VehicleDataSimulator.Application.Interfaces;
using VehicleDataSimulator.Application.Services.Generators;

namespace VehicleDataSimulator.Application.Services;

public class VehicleDataGeneratorService(
    IVehicleApiClient apiClient,
    IEnumerable<ITelemetryPayload> generators,
    ILogger<VehicleDataGeneratorService> logger)
{
    public virtual async Task ProcessVehicleTickAsync(CancellationToken cancellationToken)
    {
        var generatorList = generators.ToList();

        var vehicle = GetVehicle();

        var selectedGenerator = generatorList.FirstOrDefault(x =>
            x.VehicleType.Equals(vehicle.Brand, StringComparison.OrdinalIgnoreCase)
        );

        if (selectedGenerator is null)
        {
            throw new InvalidOperationException($"Bu marka için generator bulunamadı: {vehicle.Brand}");
        }

        var data = selectedGenerator.Generate(vehicle.Plate);

        bool success = await apiClient.SendDataAsync(data, vehicle.Plate, cancellationToken);

        LogData(success, data);
    }

    private static VehicleInfo GetVehicle()
    {
        VehicleInfo[] vehicles =
        {
            new("34BMW001", "BMW"),
            new("34BMW002", "BMW"),
            new("06BMW003", "BMW"),

            new("35MAZ001", "Mazda"),
            new("34MAZ002", "Mazda"),
            new("16MAZ003", "Mazda"),

            new("06TOY001", "Toyota"),
            new("34TOY002", "Toyota"),
            new("35TOY003", "Toyota"),
            new("41TOY004", "Toyota"),

            new("34HON001", "Honda"),

            new("06HON002", "Honda"),
            new("35HON003", "Honda"),
            new("16HON004", "Honda")
        };

        return vehicles[Random.Shared.Next(vehicles.Length)];
    }

    private void LogData(bool success, object data)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        };

        var jsonString = System.Text.Json.JsonSerializer.Serialize(data, options);

        if (success)
        {
            logger.LogInformation("Vehicle process success: {Success}. Payload: {JsonString}", success, jsonString);
        }
        else
        {
            logger.LogWarning("Vehicle process failed: {Success}. Payload: {JsonString}", success, jsonString);
        }
    }

    private record VehicleInfo(string Plate, string Brand);
}

