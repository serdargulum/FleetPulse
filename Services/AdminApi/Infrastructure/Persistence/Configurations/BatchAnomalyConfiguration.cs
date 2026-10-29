using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdminApi.Domain.Entities;

namespace AdminApi.Infrastructure.Persistence.Configurations;

public class BatchAnomalyConfiguration : IEntityTypeConfiguration<BatchAnomaly>
{
    public void Configure(EntityTypeBuilder<BatchAnomaly> builder)
    {
        builder.HasKey(ba => ba.Id);

        builder.Property(ba => ba.AnomalyType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ba => ba.Description)
            .HasMaxLength(500);

        builder.HasIndex(ba => ba.AnalysisDate);
    }
}