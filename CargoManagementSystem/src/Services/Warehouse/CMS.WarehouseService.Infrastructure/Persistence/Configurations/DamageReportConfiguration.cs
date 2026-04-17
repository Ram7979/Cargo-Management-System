using CMS.WarehouseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.WarehouseService.Infrastructure.Persistence.Configurations;

public class DamageReportConfiguration : IEntityTypeConfiguration<DamageReport>
{
    public void Configure(EntityTypeBuilder<DamageReport> builder)
    {
        builder.ToTable("DamageReports");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.ReportedByUserId).IsRequired().HasMaxLength(200);
        builder.Property(d => d.DamageNotes).IsRequired().HasMaxLength(2000);
        builder.Property(d => d.ResolutionNotes).HasMaxLength(2000);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasIndex(d => d.ShipmentId);
        builder.HasIndex(d => d.WarehouseId);
        builder.HasIndex(d => d.Status);
    }
}
