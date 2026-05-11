using CMS.IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.IdentityService.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorId)
            .HasMaxLength(256);

        builder.Property(a => a.Action)
            .HasMaxLength(100);

        builder.Property(a => a.ResourceType)
            .HasMaxLength(100);

        builder.Property(a => a.ResourceId)
            .HasMaxLength(256);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.Payload)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.Timestamp)
            .IsRequired();
    }
}
