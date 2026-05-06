using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class SubCategoryConfiguration : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.HasIndex(s => new { s.CategoryId, s.Slug })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(s => s.IsDeleted);

        builder.HasMany(s => s.Courses)
            .WithOne(c => c.SubCategory)
            .HasForeignKey(c => c.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}