using VehicleGateway.Application.Models;

namespace VehicleGateway.Application.Interfaces;

public interface IDeviceProtocolParser
{
    TelemetryData Parse(ReadOnlyMemory<byte> data);
}