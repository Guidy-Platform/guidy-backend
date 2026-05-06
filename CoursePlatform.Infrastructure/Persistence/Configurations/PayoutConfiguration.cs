using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("numeric(18,2)");
        builder.Property(p => p.PlatformFee)
            .HasColumnType("numeric(18,2)");
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(p => p.RejectionReason)
            .HasMaxLength(500);
        builder.Property(p => p.StripeTransferId)
            .HasMaxLength(100);
        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Instructor)
            .WithMany()
            .HasForeignKey(p => p.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.InstructorId);
        builder.HasIndex(p => p.Status);
    }
}