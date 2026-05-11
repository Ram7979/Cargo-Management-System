using CMS.WarehouseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.WarehouseService.Infrastructure.Persistence.Configurations;

public class CargoReceiptConfiguration : IEntityTypeConfiguration<CargoReceipt>
{
    public void Configure(EntityTypeBuilder<CargoReceipt> builder)
    {
        builder.ToTable("CargoReceipts");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TrackingNumber).IsRequired().HasMaxLength(50);
        builder.Property(r => r.ReceivedByUserId).IsRequired().HasMaxLength(200);
        builder.Property(r => r.DamageNotes).HasMaxLength(2000);
        builder.Property(r => r.Remarks).HasMaxLength(1000);

        // One receipt per shipment (unique constraint)
        builder.HasIndex(r => r.ShipmentId).IsUnique();

        // Indexes for common query patterns
        builder.HasIndex(r => r.BinId);
        builder.HasIndex(r => r.WarehouseId);
        builder.HasIndex(r => r.TrackingNumber);
    }
}
