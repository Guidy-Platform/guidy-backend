using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class InstructorWalletConfiguration
    : IEntityTypeConfiguration<InstructorWallet>
{
    public void Configure(EntityTypeBuilder<InstructorWallet> builder)
    {
        builder.HasKey(w => w.Id);

        // كل Instructor عنده wallet واحدة بس
        builder.HasIndex(w => w.InstructorId).IsUnique();

        builder.Property(w => w.TotalEarned)
            .HasColumnType("decimal(18,2)");
        builder.Property(w => w.TotalPaidOut)
            .HasColumnType("decimal(18,2)");
        builder.Property(w => w.PendingAmount)
            .HasColumnType("decimal(18,2)");
        builder.Property(w => w.AvailableBalance)
            .HasColumnType("decimal(18,2)");
        builder.Property(w => w.StripeAccountId)
            .HasMaxLength(100);

        builder.HasOne(w => w.Instructor)
            .WithMany()
            .HasForeignKey(w => w.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}