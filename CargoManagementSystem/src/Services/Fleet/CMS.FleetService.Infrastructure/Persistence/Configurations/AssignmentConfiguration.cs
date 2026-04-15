using CMS.FleetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.FleetService.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ShipmentId).IsRequired();
        builder.Property(a => a.DriverId).IsRequired();
        builder.Property(a => a.VehicleId).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.CancellationReason).HasMaxLength(500);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.DriverId);
        builder.HasIndex(a => a.VehicleId);
        builder.HasIndex(a => a.ShipmentId);
    }
}
