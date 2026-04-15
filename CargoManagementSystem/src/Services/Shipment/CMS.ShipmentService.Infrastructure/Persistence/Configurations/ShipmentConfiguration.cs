using CMS.ShipmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.ShipmentService.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(50);

        // Sender
        builder.Property(s => s.SenderName).HasMaxLength(200);
        builder.Property(s => s.OriginAddress).IsRequired().HasMaxLength(500);
        builder.Property(s => s.SenderCity).HasMaxLength(100);
        builder.Property(s => s.SenderZip).HasMaxLength(20);
        builder.Property(s => s.SenderCountry).HasMaxLength(100);
        builder.Property(s => s.SenderContact).HasMaxLength(50);

        // Recipient
        builder.Property(s => s.RecipientName).HasMaxLength(200);
        builder.Property(s => s.DestinationAddress).IsRequired().HasMaxLength(500);
        builder.Property(s => s.RecipientCity).HasMaxLength(100);
        builder.Property(s => s.RecipientZip).HasMaxLength(20);
        builder.Property(s => s.RecipientCountry).HasMaxLength(100);
        builder.Property(s => s.RecipientContact).HasMaxLength(50);

        // Cargo
        builder.Property(s => s.WeightKg).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(s => s.VolumeCbm).HasColumnType("decimal(10,3)");
        builder.Property(s => s.DeclaredValue).HasColumnType("decimal(18,2)");
        builder.Property(s => s.CargoType).HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.CargoDescription).HasMaxLength(1000);

        // Service
        builder.Property(s => s.ServiceType).HasMaxLength(100);
        builder.Property(s => s.PaymentMode).HasConversion<string>().HasMaxLength(50);

        // International / DG
        builder.Property(s => s.HsCode).HasMaxLength(20);
        builder.Property(s => s.CountryOfOrigin).HasMaxLength(100);

        // Documents
        builder.Property(s => s.BolDocumentUrl).HasMaxLength(1000);
        builder.Property(s => s.PodImageUrl).HasMaxLength(1000);
        builder.Property(s => s.PodSignatureData).HasMaxLength(2000);

        // Cancellation
        builder.Property(s => s.CancellationReason).HasMaxLength(500);
        builder.Property(s => s.LastFailureReason).HasConversion<string?>().HasMaxLength(50);

        // GPS
        builder.OwnsOne(s => s.LastKnownLocation, gps =>
        {
            gps.Property(g => g.Latitude).HasColumnName("LastLocationLatitude");
            gps.Property(g => g.Longitude).HasColumnName("LastLocationLongitude");
            gps.Property(g => g.RecordedAt).HasColumnName("LastLocationRecordedAt");
        });

        builder.HasMany(s => s.StatusHistory)
            .WithOne()
            .HasForeignKey(h => h.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
