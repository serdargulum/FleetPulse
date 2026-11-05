using AdminApi.Domain.Entities;

namespace AdminApi.Application.Contracts.Repositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}