using CMS.CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.CustomerService.Infrastructure.Persistence.Configurations;

public class KycDocumentConfiguration : IEntityTypeConfiguration<KycDocument>
{
    public void Configure(EntityTypeBuilder<KycDocument> builder)
    {
        builder.ToTable("KycDocuments");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.CustomerId)
            .IsRequired();

        builder.Property(k => k.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.BlobReference)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(k => k.UploadedAt)
            .IsRequired();
    }
}
