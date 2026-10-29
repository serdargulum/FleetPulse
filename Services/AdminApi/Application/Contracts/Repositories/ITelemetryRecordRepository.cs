using AdminApi.Domain.Entities;

namespace AdminApi.Application.Contracts.Repositories;

public interface ITelemetryRecordRepository
{
    Task<List<TelemetryRecord>> GetRecentAsync(int count, CancellationToken cancellationToken);
    Task<List<TelemetryRecord>> GetLatestByVehicleIdAsync(Guid vehicleId, int total, CancellationToken cancellationToken);
    Task<List<TelemetryRecord>> GetAllVehiclePositionsAsync(CancellationToken cancellationToken);
}