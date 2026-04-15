using CMS.FleetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.FleetService.Infrastructure.Persistence.Configurations;

public class GpsHistoryConfiguration : IEntityTypeConfiguration<GpsHistory>
{
    public void Configure(EntityTypeBuilder<GpsHistory> builder)
    {
        builder.ToTable("GpsHistories");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.VehicleId).IsRequired();
        builder.Property(g => g.Latitude).IsRequired();
        builder.Property(g => g.Longitude).IsRequired();
        builder.Property(g => g.RecordedAt).IsRequired();
        builder.HasIndex(g => g.VehicleId);
        builder.HasIndex(g => g.AssignmentId);
        builder.HasIndex(g => g.RecordedAt);
    }
}
