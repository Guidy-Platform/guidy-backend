using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(c => c.Id);

        // One certificate per student per course
        builder.HasIndex(c => new { c.StudentId, c.CourseId })
            .IsUnique();

        // Unique verification code
        builder.HasIndex(c => c.VerifyCode)
            .IsUnique();

        builder.Property(c => c.VerifyCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.StudentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CourseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.InstructorName)
            .HasMaxLength(100);

        // Relationships

        builder.HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Course)
            .WithMany()
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}