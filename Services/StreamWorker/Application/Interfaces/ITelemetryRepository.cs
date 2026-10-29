using Npgsql;
using StreamWorker.Domain.Models;

namespace StreamWorker.Application.Interfaces;

public interface ITelemetryRepository
{
    Task SaveTelemetryAndAlarmAsync(UnifiedTelemetryDto telemetry, string alarmReason, CancellationToken cancellationToken);
}