using VehicleGateway.Application.Interfaces;
using VehicleGateway.Application.Models;

namespace VehicleGateway.Infrastructure.Protocols;

public class TeltonikaParser : IDeviceProtocolParser
{
    public TelemetryData Parse(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException("Teltonika protocol parsing is not implemented.");
    }
}