using CMS.CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.CustomerService.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CustomerCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.CustomerCode).IsUnique();

        builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CompanyName).HasMaxLength(200);
        builder.Property(c => c.ContactPerson).HasMaxLength(200);

        builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.Phone).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(500);
        builder.Property(c => c.City).IsRequired().HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.Property(c => c.ZipCode).HasMaxLength(20);
        builder.Property(c => c.Country).IsRequired().HasMaxLength(100);
        builder.Property(c => c.TaxId).HasMaxLength(100);

        builder.Property(c => c.Type).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.CreditLimit).HasColumnType("decimal(18,2)");
        builder.Property(c => c.PaymentTerms).HasMaxLength(200);
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasMany(c => c.KycDocuments)
            .WithOne()
            .HasForeignKey(k => k.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
