using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence.Configurations;

public class TelemetryAlertConfiguration : IEntityTypeConfiguration<TelemetryAlert>
{
    public void Configure(EntityTypeBuilder<TelemetryAlert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AlertType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.Property(a => a.TriggerValue)
            .HasPrecision(10, 2);

        builder.HasIndex(a => a.Timestamp);
    }
}