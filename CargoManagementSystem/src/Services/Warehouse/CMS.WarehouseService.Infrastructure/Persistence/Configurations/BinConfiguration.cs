using CMS.WarehouseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.WarehouseService.Infrastructure.Persistence.Configurations;

public class BinConfiguration : IEntityTypeConfiguration<Bin>
{
    public void Configure(EntityTypeBuilder<Bin> builder)
    {
        builder.ToTable("Bins");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BinCode).IsRequired().HasMaxLength(50);
        builder.Property(b => b.CapacityKg).HasColumnType("decimal(18,2)");

        builder.HasIndex(b => new { b.WarehouseId, b.BinCode }).IsUnique();
    }
}
