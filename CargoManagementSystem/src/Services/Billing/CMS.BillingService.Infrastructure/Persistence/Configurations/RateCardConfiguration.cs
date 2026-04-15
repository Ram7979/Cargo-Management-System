using CMS.BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.BillingService.Infrastructure.Persistence.Configurations;

public class RateCardConfiguration : IEntityTypeConfiguration<RateCard>
{
    public void Configure(EntityTypeBuilder<RateCard> builder)
    {
        builder.ToTable("RateCards");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ServiceType).IsRequired().HasMaxLength(100);
        builder.Property(r => r.ZoneFrom).HasMaxLength(100);
        builder.Property(r => r.ZoneTo).HasMaxLength(100);
        builder.Property(r => r.BaseRatePerKg).HasColumnType("decimal(18,4)");
        builder.Property(r => r.FuelSurchargePercent).HasColumnType("decimal(5,2)");
        builder.Property(r => r.HandlingFeeFlat).HasColumnType("decimal(18,2)");
        builder.Property(r => r.TaxPercent).HasColumnType("decimal(5,2)");

        builder.HasIndex(r => new { r.ServiceType, r.IsActive });
    }
}
