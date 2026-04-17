using CMS.NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RecipientId).IsRequired().HasMaxLength(256);
        builder.Property(p => p.OptedOutEventTypes).HasMaxLength(2000);

        builder.HasIndex(p => p.RecipientId).IsUnique();
    }
}
