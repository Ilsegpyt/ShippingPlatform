using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Domain.Notifications;

namespace Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(
        EntityTypeBuilder<Notification> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        entity.Property(x => x.ShipmentId)
            .IsRequired(false);

        entity.Property(x => x.CustomerVoiceId)
            .IsRequired(false);

        entity.Property(x => x.IsRead)
            .IsRequired();

        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();

        entity.HasIndex(x => new
        {
            x.UserId,
            x.IsRead,
            x.CreatedAtUtc
        });
    }
}