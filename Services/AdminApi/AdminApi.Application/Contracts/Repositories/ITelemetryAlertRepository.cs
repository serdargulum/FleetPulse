using AdminApi.Domain.Entities;

namespace AdminApi.Application.Contracts.Repositories;

public interface ITelemetryAlertRepository
{
    Task<List<TelemetryAlert>> GetRecentAsync(int count, CancellationToken cancellationToken);
    Task<List<TelemetryAlert>> GetRecentByVehicleIdAsync(Guid vehicleId, int count, CancellationToken cancellationToken);
}