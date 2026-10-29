using VehicleGateway.Application.Interfaces;
using VehicleGateway.Application.Models;

namespace VehicleGateway.Infrastructure.Protocols;

public class QueclinkParser : IDeviceProtocolParser
{
    public TelemetryData Parse(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException("Queclink protocol parsing is not implemented.");
    }
}