// Infrastructure/Persistence/DbInitializer.cs
using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoursePlatform.Infrastructure.Persistence;

public static class DbInitializer
{
    private static readonly string[] Roles =
        ["Admin", "Instructor", "Student"];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider
                              .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider
                              .GetRequiredService<UserManager<AppUser>>();
        var logger = scope.ServiceProvider
                              .GetRequiredService<ILogger<AppDbContext>>();
        try
        {
            var pending = await context.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                logger.LogInformation("Applying {Count} migration(s)...", pending.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied.");
            }

            await SeedRolesAsync(roleManager, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedCategoriesAsync(context, logger);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }

    private static async Task SeedCategoriesAsync(
        AppDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync()) return;

        var jsonPath = Path.Combine(
            AppContext.BaseDirectory, "SeedData", "categories.json");

        if (!File.Exists(jsonPath)) return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var document = JsonDocument.Parse(json);
        var cats = document.RootElement.GetProperty("categories");

        foreach (var c in cats.EnumerateArray())
        {
            var name = c.GetProperty("name").GetString()!;
            var slug = SlugHelper.GenerateSlug(name);

            var category = new Category
            {
                Name = name,
                Slug = slug,
                Description = c.TryGetProperty("description", out var d)
                                ? d.GetString() : null,
                Order = c.GetProperty("order").GetInt32(),
            };

            foreach (var s in c.GetProperty("subCategories").EnumerateArray())
            {
                var subName = s.GetProperty("name").GetString()!;
                category.SubCategories.Add(new SubCategory
                {
                    Name = subName,
                    Slug = SlugHelper.GenerateSlug(subName),
                    Order = s.GetProperty("order").GetInt32(),
                });
            }

            context.Categories.Add(category);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Categories seeded successfully.");
    }
    private static async Task SeedUsersAsync(
        UserManager<AppUser> userManager,
        ILogger logger)
    {
        // قراءة الـ JSON file
        var jsonPath = Path.Combine(
            AppContext.BaseDirectory, "SeedData", "users.json");

        if (!File.Exists(jsonPath))
        {
            logger.LogWarning("Seed file not found at {Path}", jsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        var document = JsonDocument.Parse(json);
        var users = document.RootElement.GetProperty("users");

        foreach (var u in users.EnumerateArray())
        {
            var email = u.GetProperty("email").GetString()!;

            if (await userManager.FindByEmailAsync(email) is not null)
                continue;

            var user = new AppUser
            {
                FirstName = u.GetProperty("firstName").GetString()!,
                LastName = u.GetProperty("lastName").GetString()!,
                Email = email,
                UserName = email,
                EmailConfirmed = true,  // ← seed users مش محتاجين OTP
            };

            var password = u.GetProperty("password").GetString()!;
            var role = u.GetProperty("role").GetString()!;

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to seed user {Email}: {Errors}",
                    email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            await userManager.AddToRoleAsync(user, role);
            logger.LogInformation("Seeded user: {Email} [{Role}]", email, role);
        }
    }
    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager, ILogger logger)
    {
        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;

            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            logger.LogInformation("Role '{Role}' created.", role);
        }
    }
}