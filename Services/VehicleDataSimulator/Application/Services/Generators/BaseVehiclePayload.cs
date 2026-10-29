namespace VehicleDataSimulator.Application.Services.Generators;

public abstract class BaseVehiclePayload
{
    private static int _speed = 60;

    private static double _latitude = 40.97, _longitude = 29.07;

    protected (double Latitude, double Longitude) GetPosition()
    {
        if (Random.Shared.Next(0, 4) == 3)
        {
            _latitude += Random.Shared.NextDouble() * 0.001 - 0.0005;
        }

        if (Random.Shared.Next(0, 4) == 3)
        {
            _longitude += Random.Shared.NextDouble() * 0.001 - 0.0005;
        }

        return (Math.Round(_latitude, 6), Math.Round(_longitude, 6));
    }

    protected int GetSpeed()
    {
        if (Random.Shared.Next(0, 4) == 3)
        {
            return _speed;
        }

        var range = Random.Shared.Next(5, 15);

        _speed += Random.Shared.Next(-1 * range, range);
        _speed = Math.Clamp(_speed, 0, 150);

        return _speed;
    }

    protected static Dictionary<string, string> GetDtcCodes()
    {
        var codes = new Dictionary<string, string>
        {
            ["P0300"] = "Random/Multiple Cylinder Misfire Detected",
            ["P0420"] = "Catalyst System Efficiency Below Threshold",
            ["P0171"] = "System Too Lean",
            ["P0455"] = "Evaporative Emission System Leak Detected",
            ["P0128"] = "Coolant Thermostat Temperature Below Regulating Temperature"
        };

        if (Random.Shared.Next(100) >= 10)
        {
            return [];
        }

        return codes
            .Where(_ => Random.Shared.Next(100) < 30)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    protected static bool GetCheckEngineStatus(Dictionary<string, string> dtcCodes)
    {
        return dtcCodes.Count > 0;
    }

}