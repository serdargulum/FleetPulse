namespace VehicleDataSimulator.Application.Services.Generators;

public class HondaVehiclePayload : BaseVehiclePayload, ITelemetryPayload
{

    public string VehicleType => "Honda";



    public object Generate(string plate)
    {
        var dtcCodes = GetDtcCodes();
        var (latitude, longitude) = GetPosition();

        return new
        {
            plate,

            event_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),

            gps = new
            {
                lat = latitude,
                lon = longitude,
                velocity = GetSpeed(),
                direction = Random.Shared.Next(0, 360)
            },

            powertrain = new
            {
                engine_speed = Random.Shared.Next(700, 4001),
                temperature = Random.Shared.Next(80, 101),
                fuel = Random.Shared.Next(10, 101),
                voltage = Math.Round(13.5 + Random.Shared.NextDouble() * 1.2, 2)
            },

            faults = new
            {
                codes = dtcCodes,
                warning = GetCheckEngineStatus(dtcCodes)
            }
        };
    }
}