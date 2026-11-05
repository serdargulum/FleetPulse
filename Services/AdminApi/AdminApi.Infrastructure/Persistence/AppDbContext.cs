using Microsoft.EntityFrameworkCore;
using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();
    public DbSet<TelemetryAlert> TelemetryAlerts => Set<TelemetryAlert>();
    public DbSet<BatchAnomaly> BatchAnomalies => Set<BatchAnomaly>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurations klasöründeki tüm IEntityTypeConfiguration implementasyonlarını otomatik tarar ve uygular
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}