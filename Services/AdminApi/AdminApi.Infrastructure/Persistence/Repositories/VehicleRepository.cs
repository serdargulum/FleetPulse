using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminApi.Infrastructure.Persistence.Repositories;

public class VehicleRepository(AppDbContext dbContext) : IVehicleRepository
{
    public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Vehicles
            .AsNoTracking()
            .Include(x => x.TelemetryRecords)
            .ToListAsync(cancellationToken);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}