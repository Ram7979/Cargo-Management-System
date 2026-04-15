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

        builder.Property(n => n.RecipientId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Recipient)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(n => n.Subject)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(n => n.Body)
            .IsRequired();

        builder.Property(n => n.EventType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(n => n.Channel)
            .IsRequired();

        builder.Property(n => n.Status)
            .IsRequired();
    }
}
