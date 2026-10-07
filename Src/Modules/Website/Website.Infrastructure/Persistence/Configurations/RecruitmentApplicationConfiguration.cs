using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Configurations;

public class RecruitmentApplicationConfiguration
    : IEntityTypeConfiguration<RecruitmentApplication>
{
    public void Configure(EntityTypeBuilder<RecruitmentApplication> builder)
    {
        builder.ToTable("RecruitmentApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Department)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.CvFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CvFilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}