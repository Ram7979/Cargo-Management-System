using CMS.ShipmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.ShipmentService.Infrastructure.Persistence.Configurations;

public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder.ToTable("ShipmentStatusHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ShipmentId)
            .IsRequired();

        builder.Property(h => h.FromStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(h => h.ToStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(h => h.ActorId)
            .HasMaxLength(256);

        builder.Property(h => h.Notes)
            .HasMaxLength(1000);

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        builder.OwnsOne(h => h.Location, gps =>
        {
            gps.Property(g => g.Latitude).HasColumnName("LocationLatitude");
            gps.Property(g => g.Longitude).HasColumnName("LocationLongitude");
            gps.Property(g => g.RecordedAt).HasColumnName("LocationRecordedAt");
        });
    }
}
