using CMS.ReportingService.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.ReportingService.Infrastructure.Persistence.Configurations;

public class ShipmentReadModelConfiguration : IEntityTypeConfiguration<ShipmentReadModel>
{
    public void Configure(EntityTypeBuilder<ShipmentReadModel> builder)
    {
        builder.ToTable("ShipmentReadModels");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.TrackingNumber).IsUnique();

        builder.Property(s => s.CustomerCode).IsRequired().HasMaxLength(50);
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Status).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.Status);

        builder.Property(s => s.OriginAddress).IsRequired().HasMaxLength(500);
        builder.Property(s => s.OriginCity).HasMaxLength(100);
        builder.Property(s => s.OriginCountry).HasMaxLength(100);
        builder.Property(s => s.DestinationAddress).IsRequired().HasMaxLength(500);
        builder.Property(s => s.DestinationCity).HasMaxLength(100);
        builder.Property(s => s.DestinationCountry).HasMaxLength(100);

        builder.Property(s => s.WeightKg).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ServiceType).HasMaxLength(100);
        builder.Property(s => s.CargoType).HasMaxLength(100);

        builder.Property(s => s.DriverId).HasMaxLength(200);
        builder.Property(s => s.DriverName).HasMaxLength(200);
        builder.Property(s => s.VehicleId).HasMaxLength(200);
        builder.Property(s => s.PlateNumber).HasMaxLength(50);

        builder.Property(s => s.InvoiceAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(s => s.CreatedAt);
        builder.HasIndex(s => s.CustomerCode);
        builder.HasIndex(s => s.DriverId);
        builder.HasIndex(s => s.CargoType);
        builder.HasIndex(s => new { s.OriginCity, s.OriginCountry });
        builder.HasIndex(s => new { s.DestinationCity, s.DestinationCountry });
    }
}
