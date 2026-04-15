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

        builder.Property(r => r.ReceivedByUserId).IsRequired().HasMaxLength(200);
        builder.Property(r => r.DamageNotes).HasMaxLength(2000);
    }
}
