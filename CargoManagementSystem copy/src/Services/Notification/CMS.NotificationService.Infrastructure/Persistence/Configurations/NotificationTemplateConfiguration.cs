using CMS.NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.EventType).IsRequired().HasMaxLength(128);
        builder.Property(t => t.Channel).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.SubjectTemplate).IsRequired().HasMaxLength(512);
        builder.Property(t => t.BodyTemplate).IsRequired();

        builder.HasIndex(t => new { t.EventType, t.Channel }).IsUnique();
    }
}
