
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;
using Polly.Registry;
using StreamWorker.Domain.Models;
using StreamWorker.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace StreamWorker.Tests.Integration.Persistence;

public class TelemetryRepositoryIntegrationTests
{
    [Fact]
    public async Task SaveTelemetryAndAlarmAsync_ValidTelemetry_SavesRecord()
    {
        // Test için izole PostgreSQL konteyneri başlatır.
        await using var postgres = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("fleetpulse_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        await postgres.StartAsync();

        var connectionString = postgres.GetConnectionString();

        // Repository'nin ihtiyaç duyduğu tabloları oluşturur.
        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var sql = """
                CREATE TABLE "Vehicles" (
                    "Id" uuid PRIMARY KEY,
                    "Plate" varchar(20) NOT NULL UNIQUE
                );

                CREATE TABLE "TelemetryRecords" (
                    "Id" uuid PRIMARY KEY,
                    "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                    "Latitude" numeric(10,8) NOT NULL,
                    "Longitude" numeric(11,8) NOT NULL,
                    "Speed" numeric(5,2) NOT NULL,
                    "EngineTemperature" numeric(5,2) NOT NULL,
                    "FuelLevel" double precision NOT NULL,
                    "RecordedAt" timestamp with time zone NOT NULL
                );

                CREATE TABLE "TelemetryAlerts" (
                    "Id" uuid PRIMARY KEY,
                    "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                    "AlertType" varchar(100) NOT NULL,
                    "Description" varchar(500) NOT NULL,
                    "TriggerValue" numeric(10,2) NOT NULL,
                    "Timestamp" timestamp with time zone NOT NULL
                );
                """;

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();

            var vehicleId = Guid.NewGuid();

            await using var insertVehicle = new NpgsqlCommand(
                """INSERT INTO "Vehicles" ("Id", "Plate") VALUES (@Id, @Plate)""",
                connection);

            insertVehicle.Parameters.AddWithValue("Id", vehicleId);
            insertVehicle.Parameters.AddWithValue("Plate", "34TEST001");

            await insertVehicle.ExecuteNonQueryAsync();
        }

        // Repository için bağlantı ayarlarını ve Polly pipeline'ını yapılandırır.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();

        using var loggerFactory = LoggerFactory.Create(_ => { });

        var services = new ServiceCollection();

        services.AddResiliencePipeline("postgres-retry", pipeline =>
        {
            pipeline.AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder()
                    .Handle<NpgsqlException>(ex => ex.IsTransient)
            });
        });

        using var provider = services.BuildServiceProvider();

        var pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();

        var repository = new TelemetryRepository(
            configuration,
            loggerFactory.CreateLogger<TelemetryRepository>(),
            pipelineProvider);

        var telemetry = new UnifiedTelemetryDto
        {
            Plate = "34TEST001",
            Latitude = 41.0082m,
            Longitude = 28.9784m,
            Speed = 60m,
            EngineTemperature = 90m,
            FuelLevel = 75,
            RecordedAt = DateTime.UtcNow
        };

        // Telemetriyi kaydeder ve veritabanındaki kaydı doğrular.
        await repository.SaveTelemetryAndAlarmAsync(telemetry, "active", CancellationToken.None);

        await using var verifyConnection = new NpgsqlConnection(connectionString);
        await verifyConnection.OpenAsync();

        await using var verifyCommand = new NpgsqlCommand(
            """SELECT COUNT(*) FROM "TelemetryRecords" WHERE "VehicleId" = @VehicleId""",
            verifyConnection);

        verifyCommand.Parameters.AddWithValue("VehicleId", await GetVehicleId(verifyConnection, "34TEST001"));

        var count = (long)(await verifyCommand.ExecuteScalarAsync())!;

        Assert.Equal(1, count);
    }

    private static async Task<Guid> GetVehicleId(NpgsqlConnection connection, string plate)
    {
        await using var command = new NpgsqlCommand(
            """SELECT "Id" FROM "Vehicles" WHERE "Plate" = @Plate""",
            connection);

        command.Parameters.AddWithValue("Plate", plate);

        return (Guid)(await command.ExecuteScalarAsync())!;
    }


    [Fact]
    public async Task SaveTelemetryAndAlarmAsync_TelemetryHasErrors_SavesTelemetryAndAlert()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("fleetpulse_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        await postgres.StartAsync();

        var connectionString = postgres.GetConnectionString();

        // Repository'nin ihtiyaç duyduğu tabloları oluşturur.
        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var sql = """
            CREATE TABLE "Vehicles" (
                "Id" uuid PRIMARY KEY,
                "Plate" varchar(20) NOT NULL UNIQUE
            );

            CREATE TABLE "TelemetryRecords" (
                "Id" uuid PRIMARY KEY,
                "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                "Latitude" numeric(10,8) NOT NULL,
                "Longitude" numeric(11,8) NOT NULL,
                "Speed" numeric(5,2) NOT NULL,
                "EngineTemperature" numeric(5,2) NOT NULL,
                "FuelLevel" double precision NOT NULL,
                "RecordedAt" timestamp with time zone NOT NULL
            );

            CREATE TABLE "TelemetryAlerts" (
                "Id" uuid PRIMARY KEY,
                "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                "AlertType" varchar(100) NOT NULL,
                "Description" varchar(500) NOT NULL,
                "TriggerValue" numeric(10,2) NOT NULL,
                "Timestamp" timestamp with time zone NOT NULL
            );
            """;

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();

            await using var insertVehicle = new NpgsqlCommand(
                """INSERT INTO "Vehicles" ("Id", "Plate") VALUES (@Id, @Plate)""",
                connection);

            insertVehicle.Parameters.AddWithValue("Id", Guid.NewGuid());
            insertVehicle.Parameters.AddWithValue("Plate", "34TEST002");

            await insertVehicle.ExecuteNonQueryAsync();
        }

        // Repository bağımlılıklarını yapılandırır.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();

        using var loggerFactory = LoggerFactory.Create(_ => { });
        var services = new ServiceCollection();

        services.AddResiliencePipeline("postgres-retry", pipeline =>
        {
            pipeline.AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder()
                    .Handle<NpgsqlException>(ex => ex.IsTransient)
            });
        });

        using var provider = services.BuildServiceProvider();

        var repository = new TelemetryRepository(
            configuration,
            loggerFactory.CreateLogger<TelemetryRepository>(),
            provider.GetRequiredService<ResiliencePipelineProvider<string>>());

        var telemetry = new UnifiedTelemetryDto
        {
            Plate = "34TEST002",
            Latitude = 41.0082m,
            Longitude = 28.9784m,
            Speed = 60m,
            EngineTemperature = 110m,
            FuelLevel = 50,
            RecordedAt = DateTime.UtcNow,
            ErrorList = new Dictionary<string, string>
            {
                ["HighEngineTemperature"] = "Motor sıcaklığı sınırı aştı."
            }
        };

        // Telemetriyi ve ilişkili alarmı kaydeder.
        await repository.SaveTelemetryAndAlarmAsync(
            telemetry,
            "active",
            CancellationToken.None);

        await using var verifyConnection = new NpgsqlConnection(connectionString);
        await verifyConnection.OpenAsync();

        // Telemetri ve alarm kayıtlarının ikisinin de oluştuğunu doğrular.
        await using var verifyCommand = new NpgsqlCommand(
            """
        SELECT
            (SELECT COUNT(*) FROM "TelemetryRecords" WHERE "VehicleId" = @VehicleId),
            (SELECT COUNT(*) FROM "TelemetryAlerts" WHERE "VehicleId" = @VehicleId)
        """,
            verifyConnection);

        verifyCommand.Parameters.AddWithValue(
            "VehicleId",
            await GetVehicleId(verifyConnection, "34TEST002"));

        await using var reader = await verifyCommand.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(1L, reader.GetInt64(0));
        Assert.Equal(1L, reader.GetInt64(1));
    }


    [Fact]
    public async Task SaveTelemetryAndAlarmAsync_VehicleNotFound_ThrowsException()
    {
        // İzole PostgreSQL konteynerini başlatır.
        await using var postgres = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("fleetpulse_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        await postgres.StartAsync();

        var connectionString = postgres.GetConnectionString();

        // Repository'nin ihtiyaç duyduğu tabloları oluşturur.
        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var sql = """
            CREATE TABLE "Vehicles" (
                "Id" uuid PRIMARY KEY,
                "Plate" varchar(20) NOT NULL UNIQUE
            );

            CREATE TABLE "TelemetryRecords" (
                "Id" uuid PRIMARY KEY,
                "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                "Latitude" numeric(10,8) NOT NULL,
                "Longitude" numeric(11,8) NOT NULL,
                "Speed" numeric(5,2) NOT NULL,
                "EngineTemperature" numeric(5,2) NOT NULL,
                "FuelLevel" double precision NOT NULL,
                "RecordedAt" timestamp with time zone NOT NULL
            );

            CREATE TABLE "TelemetryAlerts" (
                "Id" uuid PRIMARY KEY,
                "VehicleId" uuid NOT NULL REFERENCES "Vehicles"("Id"),
                "AlertType" varchar(100) NOT NULL,
                "Description" varchar(500) NOT NULL,
                "TriggerValue" numeric(10,2) NOT NULL,
                "Timestamp" timestamp with time zone NOT NULL
            );
            """;

            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        // Repository bağımlılıklarını yapılandırır.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();

        using var loggerFactory = LoggerFactory.Create(_ => { });
        var services = new ServiceCollection();

        services.AddResiliencePipeline("postgres-retry", pipeline =>
        {
            pipeline.AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder()
                    .Handle<NpgsqlException>(ex => ex.IsTransient)
            });
        });

        using var provider = services.BuildServiceProvider();

        var repository = new TelemetryRepository(
            configuration,
            loggerFactory.CreateLogger<TelemetryRepository>(),
            provider.GetRequiredService<ResiliencePipelineProvider<string>>());

        var telemetry = new UnifiedTelemetryDto
        {
            Plate = "34MISSING",
            Latitude = 41.0082m,
            Longitude = 28.9784m,
            Speed = 60m,
            EngineTemperature = 90m,
            FuelLevel = 75,
            RecordedAt = DateTime.UtcNow
        };

        // Bulunmayan araç için kayıt işleminin hata verdiğini doğrular.
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.SaveTelemetryAndAlarmAsync(
                telemetry,
                "active",
                CancellationToken.None));

        Assert.Contains("Araç bulunamadı", exception.Message);
    }

}
