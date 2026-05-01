using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class PlatformSettingConfiguration
    : IEntityTypeConfiguration<PlatformSetting>
{
    public void Configure(EntityTypeBuilder<PlatformSetting> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Key)
            .IsRequired().HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired().HasMaxLength(2000);

        builder.Property(s => s.Group)
            .HasMaxLength(50);

        // key must be unique
        builder.HasIndex(s => s.Key).IsUnique();
    }
}