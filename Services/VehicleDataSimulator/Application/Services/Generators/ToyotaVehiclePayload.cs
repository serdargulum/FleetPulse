namespace VehicleDataSimulator.Application.Services.Generators;

public class ToyotaVehiclePayload : BaseVehiclePayload, ITelemetryPayload
{
    public string VehicleType => "toyota";

    public object Generate(string plate)
    {
        var dtcCodes = GetDtcCodes();
        var (latitude, longitude) = GetPosition();

        return new
        {
            plate,
            timestamp = DateTime.UtcNow,

            latitude,
            longitude,
            speed = GetSpeed(),

            rpm = Random.Shared.Next(700, 4001),
            coolantTemp = Random.Shared.Next(80, 101),
            fuelLevelPercentage = Random.Shared.Next(10, 101),
            batteryVoltage = Math.Round(13.5 + Random.Shared.NextDouble() * 1.2, 1),

            dtcCodes,
            isCheckEngineOn = GetCheckEngineStatus(dtcCodes)
        };
    }
}