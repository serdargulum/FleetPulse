using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence.Configurations;

public class TelemetryRecordConfiguration : IEntityTypeConfiguration<TelemetryRecord>
{
    public void Configure(EntityTypeBuilder<TelemetryRecord> builder)
    {
        builder.HasKey(tr => new { tr.Id, tr.RecordedAt });

        builder.Property(tr => tr.Speed)
            .HasPrecision(5, 2);

        builder.Property(tr => tr.EngineTemperature)
            .HasPrecision(5, 2);

        builder.HasIndex(tr => tr.RecordedAt);

        builder.Property(e => e.Location)
            .HasColumnType("geography(Point,4326)");
    }
}