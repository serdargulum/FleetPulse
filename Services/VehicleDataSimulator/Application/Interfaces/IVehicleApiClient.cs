using System.Threading;
using System.Threading.Tasks;

namespace VehicleDataSimulator.Application.Interfaces;

public interface IVehicleApiClient
{
    Task<bool> SendDataAsync(Object data, string plate, CancellationToken cancellationToken);
}