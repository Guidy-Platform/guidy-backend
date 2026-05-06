using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        // Basic fields
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Order)
            .IsRequired();

        // Soft delete
        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(c => c.IsDeleted);

        //  IMPORTANT: PostgreSQL filtered unique index (soft delete safe)
        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        // Relationship
        builder.HasMany(c => c.SubCategories)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}