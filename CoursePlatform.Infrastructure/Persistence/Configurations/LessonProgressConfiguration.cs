using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(lp => lp.Id);


        builder.HasIndex(lp => new { lp.StudentId, lp.CourseId });

        builder.HasOne(lp => lp.Student)
            .WithMany()
            .HasForeignKey(lp => lp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lp => lp.Lesson)
            .WithMany()
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(lp => lp.Course)
            .WithMany()
            .HasForeignKey(lp => lp.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}