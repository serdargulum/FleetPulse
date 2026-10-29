using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByPlateAsync(string plate, CancellationToken cancellationToken);

}