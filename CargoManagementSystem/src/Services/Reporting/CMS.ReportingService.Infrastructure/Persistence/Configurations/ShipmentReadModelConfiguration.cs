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

        builder.Property(s => s.TrackingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.TrackingNumber)
            .IsUnique();

        builder.Property(s => s.CustomerCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.Status);

        builder.Property(s => s.OriginAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.DestinationAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.WeightKg)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ServiceType)
            .HasMaxLength(100);

        builder.HasIndex(s => s.CreatedAt);
    }
}
