using CMS.FleetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.FleetService.Infrastructure.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.LicenseNumber).IsUnique();
        builder.HasIndex(d => d.EmployeeId).IsUnique();

        builder.Property(d => d.EmployeeId).IsRequired().HasMaxLength(50);
        builder.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Phone).HasMaxLength(20);
        builder.Property(d => d.ProfilePhotoUrl).HasMaxLength(1000);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(50);
    }
}
