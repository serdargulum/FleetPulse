using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StreamWorker.Domain.Models;
using StreamWorker.Application.Interfaces;
using Polly;
using Polly.Registry;

namespace StreamWorker.Infrastructure.Persistence;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly string _connectionString;
    private readonly ILogger<TelemetryRepository> _logger;

    private readonly ResiliencePipeline _pipeline;

    public TelemetryRepository(
        IConfiguration configuration,
        ILogger<TelemetryRepository> logger,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string bulunamadı!");
        _logger = logger;
        _pipeline = pipelineProvider.GetPipeline("postgres-retry");
    }

    private async Task<Guid> GetVehicleId(string Plate, NpgsqlConnection connection, NpgsqlTransaction transaction, CancellationToken cancellationToken)
    {
        var lookupSql = @"SELECT ""Id"" FROM ""Vehicles"" WHERE ""Plate"" = @Plate LIMIT 1";

        using (var lookupCmd = new NpgsqlCommand(lookupSql, connection, transaction))
        {
            lookupCmd.Parameters.AddWithValue("Plate", Plate);

            var result = await lookupCmd.ExecuteScalarAsync(cancellationToken);
            if (result != null && result != DBNull.Value)
            {
                return (Guid)result;
            }
        }

        throw new InvalidOperationException($"Araç bulunamadı! Plaka veya DeviceId sistemde kayıtlı değil.");
    }

    public async Task SaveTelemetryAndAlarmAsync(
        UnifiedTelemetryDto telemetryRecord,
        string alarmReason,
        CancellationToken cancellationToken)
    {
        await _pipeline.ExecuteAsync(
        async ct =>
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            await using var transaction = await connection.BeginTransactionAsync(ct);

            try
            {
                if (string.IsNullOrWhiteSpace(telemetryRecord.Plate))
                {
                    throw new InvalidOperationException("Plaka bilgisi eksik.");
                }

                var vehicleId = await GetVehicleId(telemetryRecord.Plate, connection, transaction, ct);

                await SaveTelemetry(telemetryRecord, vehicleId, connection, transaction, ct);

                if (telemetryRecord.ErrorList.Count > 0)
                {
                    await SaveTelemetryAlert(telemetryRecord, vehicleId, connection, transaction, ct);
                }

                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError(ex, "Telemetri ve alarm PostgreSQL'e yazılırken hata oluştu.");
                throw;
            }
        }, cancellationToken);
    }


    private async Task SaveTelemetry(
        UnifiedTelemetryDto telemetryRecord,
        Guid vehicleId,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO "TelemetryRecords"
            ("Id", "VehicleId", "Location", "Speed", "EngineTemperature", "FuelLevel", "RecordedAt")
            VALUES (
                @Id, @VehicleId,
                ST_SetSRID(ST_MakePoint(@Longitude, @Latitude), 4326)::geography,
                @Speed, @EngineTemperature, @FuelLevel, @RecordedAt
            )
            """;

        await using var cmd = new NpgsqlCommand(sql, connection, transaction);
        cmd.Parameters.AddWithValue("Id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("VehicleId", vehicleId);
        cmd.Parameters.AddWithValue("Latitude", telemetryRecord.Latitude);
        cmd.Parameters.AddWithValue("Longitude", telemetryRecord.Longitude);
        cmd.Parameters.AddWithValue("Speed", telemetryRecord.Speed);
        cmd.Parameters.AddWithValue("EngineTemperature", telemetryRecord.EngineTemperature);
        cmd.Parameters.AddWithValue("FuelLevel", telemetryRecord.FuelLevel);
        cmd.Parameters.AddWithValue(
            "RecordedAt",
            telemetryRecord.RecordedAt == default
                ? DateTime.UtcNow
                : telemetryRecord.RecordedAt);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }


    private async Task SaveTelemetryAlert(UnifiedTelemetryDto telemetryRecord, Guid vehicleId, NpgsqlConnection connection, NpgsqlTransaction transaction, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO "TelemetryAlerts" ("Id", "VehicleId", "AlertType", "Description", "TriggerValue", "Timestamp")
            VALUES (@Id, @VehicleId, @AlertType, @Description, @TriggerValue, @Timestamp)
        """;

        foreach (var error in telemetryRecord.ErrorList)
        {
            await using var cmd = new NpgsqlCommand(sql, connection, transaction);
            cmd.Parameters.AddWithValue("Id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("VehicleId", vehicleId);
            cmd.Parameters.AddWithValue("AlertType", error.Key);
            cmd.Parameters.AddWithValue("Description", error.Value);
            cmd.Parameters.AddWithValue("TriggerValue", telemetryRecord.EngineTemperature);
            cmd.Parameters.AddWithValue("Timestamp", telemetryRecord.RecordedAt == default ? DateTime.UtcNow : telemetryRecord.RecordedAt);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }


}