using CMS.FleetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.FleetService.Infrastructure.Persistence.Configurations;

public class MaintenanceLogConfiguration : IEntityTypeConfiguration<MaintenanceLog>
{
    public void Configure(EntityTypeBuilder<MaintenanceLog> builder)
    {
        builder.ToTable("MaintenanceLogs");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.VehicleId).IsRequired();
        builder.Property(m => m.MaintenanceType).IsRequired().HasMaxLength(200);
        builder.Property(m => m.ServiceProvider).HasMaxLength(200);
        builder.Property(m => m.Notes).HasMaxLength(1000);
        builder.Property(m => m.Cost).HasColumnType("decimal(18,2)");
        builder.Property(m => m.MileageAtService).HasColumnType("decimal(10,2)");
        builder.Property(m => m.NextServiceMileage).HasColumnType("decimal(10,2)");
        builder.HasIndex(m => m.VehicleId);
    }
}
