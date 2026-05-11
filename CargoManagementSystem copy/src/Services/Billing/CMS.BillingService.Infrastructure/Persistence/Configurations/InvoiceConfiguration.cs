using CMS.BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.BillingService.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.ShipmentId).IsUnique();
        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => i.Status);

        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(i => i.BaseFreightCharge).HasColumnType("decimal(18,2)");
        builder.Property(i => i.FuelSurcharge).HasColumnType("decimal(18,2)");
        builder.Property(i => i.HandlingFee).HasColumnType("decimal(18,2)");
        builder.Property(i => i.InsuranceAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TaxRate).HasColumnType("decimal(5,4)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.OutstandingBalance).HasColumnType("decimal(18,2)");
        builder.Property(i => i.PdfBlobUrl).HasMaxLength(1000);
        builder.Property(i => i.Notes).HasMaxLength(1000);
        builder.Property(i => i.VoidReason).HasMaxLength(500);

        builder.HasMany(i => i.Payments)
            .WithOne()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
