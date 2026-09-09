using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Domain.Outbox;

namespace Notifications.Infrastructure.Persistence.Configurations;

public sealed class EmailOutboxMessageConfiguration
    : IEntityTypeConfiguration<EmailOutboxMessage>
{
    public void Configure(
        EntityTypeBuilder<EmailOutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecipientEmail)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Body)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.Property(x => x.RetryCount)
            .IsRequired();
    }
}