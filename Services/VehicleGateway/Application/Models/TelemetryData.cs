namespace VehicleGateway.Application.Models;

public record TelemetryData(string DeviceId, DateTime Timestamp, double Latitude, double Longitude, double Speed, bool IgnitionOn);