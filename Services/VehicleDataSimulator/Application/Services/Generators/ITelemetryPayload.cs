namespace VehicleDataSimulator.Application.Services.Generators;

public interface ITelemetryPayload
{
    string VehicleType { get; }

    object Generate(string plate);
}