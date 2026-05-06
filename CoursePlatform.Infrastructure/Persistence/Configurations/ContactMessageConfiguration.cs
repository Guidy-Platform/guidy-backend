using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class ContactMessageConfiguration
    : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(c => c.AdminReply)
            .HasMaxLength(5000);

        //  PostgreSQL-safe enum handling
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.IpAddress)
            .HasMaxLength(50);

        // Indexes (PostgreSQL compatible)
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.CreatedAt);
    }
}