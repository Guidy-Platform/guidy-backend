using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired();

        builder.Property(c => c.ShortDescription)
            .HasMaxLength(500);

        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.DiscountPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Level)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Language)
            .HasMaxLength(50)
            .HasDefaultValue("English");

        builder.Property(c => c.Requirements)
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.WhatYouLearn)
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(1000);

        // FK → AppUser (Guid)
        builder.HasOne(c => c.Instructor)
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK → SubCategory (int)
        builder.HasOne(c => c.SubCategory)
            .WithMany()
            .HasForeignKey(c => c.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes للـ common queries
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.InstructorId);
        builder.HasIndex(c => c.SubCategoryId);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => new { c.Status, c.IsDeleted });
    }
}