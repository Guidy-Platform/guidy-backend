using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoursePlatform.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.DiscountValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        //  PostgreSQL filtered unique index
        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}