using CMS.BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.BillingService.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(p => p.ReferenceNumber).HasMaxLength(200);
        builder.Property(p => p.ReceiptBlobUrl).HasMaxLength(1000);
        builder.Property(p => p.RecordedBy).HasMaxLength(256);
        builder.Property(p => p.Notes).HasMaxLength(1000);
        builder.Property(p => p.RefundReason).HasMaxLength(500);

        builder.HasIndex(p => p.InvoiceId);
        builder.HasIndex(p => p.PaidAt);
    }
}
