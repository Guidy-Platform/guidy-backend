using CoursePlatform.Domain.Common.Interfaces;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoursePlatform.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly AuditInterceptor _auditInterceptor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditInterceptor auditInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    // DbSets add with each  feature
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // apply all configurations from the assembly containing AppDbContext implement IEntityTypeConfiguration<TEntity>
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global Soft Delete Filter — ISoftDelete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                continue;

            var param = Expression.Parameter(entityType.ClrType, "e");
            var prop = Expression.Property(param, nameof(ISoftDelete.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(prop), param);

            builder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        // use simple names for Identity tables
        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}