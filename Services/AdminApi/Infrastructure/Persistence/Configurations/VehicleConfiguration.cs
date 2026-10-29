using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Plate)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(v => v.Plate)
            .IsUnique();

        builder.Property(v => v.Brand)
            .HasMaxLength(50);

        builder.Property(v => v.Model)
            .HasMaxLength(50);

        // İlişkiler
        builder.HasMany(v => v.Alerts)
            .WithOne(a => a.Vehicle)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.TelemetryRecords)
            .WithOne(tr => tr.Vehicle)
            .HasForeignKey(tr => tr.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.BatchAnomalies)
            .WithOne(ba => ba.Vehicle)
            .HasForeignKey(ba => ba.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}