using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminApi.Infrastructure.Persistence.Repositories;

public class TelemetryAlertRepository(AppDbContext dbContext) : ITelemetryAlertRepository
{

    public async Task<List<TelemetryAlert>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken)
    {
        return await dbContext.TelemetryAlerts
            .AsNoTracking()
            .OrderByDescending(x => x.Timestamp)
            .Include(x => x.Vehicle)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TelemetryAlert>> GetRecentByVehicleIdAsync(
        Guid vehicleId,
        int count,
        CancellationToken cancellationToken)
    {
        return await dbContext.TelemetryAlerts
            .AsNoTracking()
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}