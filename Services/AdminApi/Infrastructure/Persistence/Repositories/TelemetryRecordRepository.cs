
using AdminApi.Application.Contracts.Repositories;
using AdminApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminApi.Infrastructure.Persistence.Repositories;

public class TelemetryRecordRepository(AppDbContext dbContext) : ITelemetryRecordRepository
{
    public async Task<List<TelemetryRecord>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken)
    {
        return await dbContext.TelemetryRecords
            .AsNoTracking()
            .OrderByDescending(x => x.RecordedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TelemetryRecord>> GetLatestByVehicleIdAsync(
        Guid vehicleId,
        int total,
        CancellationToken cancellationToken)
    {
        return await dbContext.TelemetryRecords
            .AsNoTracking()
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.RecordedAt)
            .Take(total)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TelemetryRecord>> GetAllVehiclePositionsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TelemetryRecords
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .GroupBy(x => x.VehicleId)
            .Select(g => g.OrderByDescending(x => x.RecordedAt).First())
            .ToListAsync(cancellationToken);
    }
}