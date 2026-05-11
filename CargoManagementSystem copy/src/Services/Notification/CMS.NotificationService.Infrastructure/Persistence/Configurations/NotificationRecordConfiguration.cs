using CMS.NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationRecordConfiguration : IEntityTypeConfiguration<NotificationRecord>
{
    public void Configure(EntityTypeBuilder<NotificationRecord> builder)
    {
        builder.ToTable("NotificationRecords");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.RecipientId).IsRequired().HasMaxLength(256);
        builder.Property(n => n.Recipient).IsRequired().HasMaxLength(512);
        builder.Property(n => n.Subject).IsRequired().HasMaxLength(512);
        builder.Property(n => n.Body).IsRequired();
        builder.Property(n => n.EventType).IsRequired().HasMaxLength(128);
        builder.Property(n => n.Channel).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.ErrorMessage).HasMaxLength(2000);

        // Soft-delete filter — exclude deleted records from all queries
        builder.HasQueryFilter(n => !n.IsDeleted);

        builder.HasIndex(n => n.RecipientId);
        builder.HasIndex(n => n.Status);
        builder.HasIndex(n => n.Channel);
        builder.HasIndex(n => n.EventType);
        builder.HasIndex(n => n.CreatedAt);
    }
}
