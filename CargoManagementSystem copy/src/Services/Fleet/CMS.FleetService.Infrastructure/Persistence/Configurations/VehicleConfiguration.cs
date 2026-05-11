using CMS.FleetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.FleetService.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);
        builder.HasIndex(v => v.PlateNumber).IsUnique();

        builder.Property(v => v.PlateNumber).IsRequired().HasMaxLength(20);
        builder.Property(v => v.GpsDeviceId).HasMaxLength(100);
        builder.Property(v => v.Make).HasMaxLength(100);
        builder.Property(v => v.Model).HasMaxLength(100);
        builder.Property(v => v.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.FuelType).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.GpsTrackingStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.CapacityKg).HasColumnType("decimal(18,2)");
        builder.Property(v => v.VolumeCbm).HasColumnType("decimal(10,3)");
        builder.Property(v => v.CurrentLoadKg).HasColumnType("decimal(18,2)");
        builder.Property(v => v.RegistrationCertificateUrl).HasMaxLength(1000);
        builder.Property(v => v.InsuranceDocumentUrl).HasMaxLength(1000);

        builder.OwnsOne(v => v.LastLocation, gps =>
        {
            gps.Property(g => g.Latitude).HasColumnName("LastLocationLatitude");
            gps.Property(g => g.Longitude).HasColumnName("LastLocationLongitude");
            gps.Property(g => g.RecordedAt).HasColumnName("LastLocationRecordedAt");
        });

        builder.HasMany(v => v.MaintenanceLogs)
            .WithOne()
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
