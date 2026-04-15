using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.WarehouseService.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Domain.Entities.Warehouse>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Address).IsRequired().HasMaxLength(500);
        builder.Property(w => w.City).IsRequired().HasMaxLength(100);
        builder.Property(w => w.Country).IsRequired().HasMaxLength(100);

        builder.HasMany(w => w.Bins)
            .WithOne()
            .HasForeignKey(b => b.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
