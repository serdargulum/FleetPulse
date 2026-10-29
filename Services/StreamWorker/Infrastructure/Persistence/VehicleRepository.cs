using Npgsql;
using StreamWorker.Domain.Models;
using StreamWorker.Application.Interfaces;

namespace StreamWorker.Infrastructure.Persistence;

public class VehicleRepository(
    IConfiguration configuration,
    ILogger<VehicleRepository> logger) : IVehicleRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string bulunamadı!");

    public async Task<Vehicle?> GetByPlateAsync(string plate, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                "Id",
                "Plate",
                "Brand"
            FROM "Vehicles"
            WHERE "Plate" = @Plate
            LIMIT 1
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        try
        {
            await using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("Plate", plate);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Vehicle
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Plate = reader.GetString(reader.GetOrdinal("Plate")),
                Brand = reader.GetString(reader.GetOrdinal("Brand"))
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Plaka ile araç aranırken hata oluştu. Plate: {Plate}",
                plate);

            throw;
        }
    }

}