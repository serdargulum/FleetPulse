namespace VehicleDataSimulator.Application.Services.Generators;

public class MazdaVehiclePayload : BaseVehiclePayload, ITelemetryPayload
{
    public string VehicleType => "mazda";

    public object Generate(string plate)
    {
        var dtcCodes = GetDtcCodes();
        var (latitude, longitude) = GetPosition();

        return new
        {
            plate,

            recordedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),

            position = new
            {
                lat = latitude,
                lng = longitude,
                kmh = GetSpeed(),
                course = Random.Shared.Next(0, 360)
            },

            motor = new
            {
                rev = Random.Shared.Next(700, 4001),
                tempC = Random.Shared.Next(80, 101),
                fuelPct = Random.Shared.Next(10, 101),
                voltageV = Math.Round(13.5 + Random.Shared.NextDouble() * 1.2, 2)
            },

            alerts = new
            {
                errorCodes = dtcCodes,
                checkEngine = GetCheckEngineStatus(dtcCodes)
            }
        };
    }
}