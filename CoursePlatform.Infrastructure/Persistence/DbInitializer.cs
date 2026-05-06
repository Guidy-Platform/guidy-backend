using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
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
            await SeedCouponsAsync(context, logger);
            await SeedCoursesAsync(context, userManager, logger);
            await SeedPlatformSettingsAsync(context, logger);


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

                IconUrl = c.TryGetProperty("iconUrl", out var ic)
                 ? ic.GetString()
                 : null
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


    
    private static async Task SeedCouponsAsync(
        AppDbContext context, ILogger logger)
    {
        if (await context.Coupons.AnyAsync()) return;

        var jsonPath = Path.Combine(
            AppContext.BaseDirectory, "SeedData", "coupons.json");

        if (!File.Exists(jsonPath)) return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var document = JsonDocument.Parse(json);
        var coupons = document.RootElement.GetProperty("coupons");

        foreach (var c in coupons.EnumerateArray())
        {
            var discountTypeStr = c.GetProperty("discountType").GetString()!;
            var discountType = Enum.Parse<DiscountType>(discountTypeStr);

            DateTime? expiresAt = null;
            if (c.TryGetProperty("expiresAt", out var exp) &&
                exp.ValueKind != JsonValueKind.Null)
                expiresAt = exp.GetDateTime();

            int? usageLimit = null;
            if (c.TryGetProperty("usageLimit", out var ul) &&
                ul.ValueKind != JsonValueKind.Null)
                usageLimit = ul.GetInt32();

            context.Coupons.Add(new Coupon
            {
                Code = c.GetProperty("code").GetString()!,
                DiscountType = discountType,
                DiscountValue = c.GetProperty("discountValue").GetDecimal(),
                UsageLimit = usageLimit,
                ExpiresAt = expiresAt,
                IsActive = c.GetProperty("isActive").GetBoolean()
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Coupons seeded successfully.");
    }



    private static async Task SeedCoursesAsync(
        AppDbContext context,
        UserManager<AppUser> userManager,
        ILogger logger)
    {
        if (await context.Courses.AnyAsync()) return;

        var jsonPath = Path.Combine(
            AppContext.BaseDirectory, "SeedData", "courses.json");

        if (!File.Exists(jsonPath))
        {
            logger.LogWarning("courses.json not found at {Path}", jsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        var document = JsonDocument.Parse(json);
        var courses = document.RootElement.GetProperty("courses");

        var totalCourses = 0;
        var totalSections = 0;
        var totalLessons = 0;

        foreach (var c in courses.EnumerateArray())
        {
            // 1. fetch instructor 
            var instructorEmail = c.GetProperty("instructorEmail").GetString()!;
            var instructor = await userManager.FindByEmailAsync(instructorEmail);
            if (instructor is null)
            {
                logger.LogWarning(
                    "Instructor '{Email}' not found. Skipping course '{Title}'.",
                    instructorEmail,
                    c.GetProperty("title").GetString());
                continue;
            }

            // SubCategory by slug
            var subCategorySlug = c.GetProperty("subCategorySlug").GetString()!;
            var subCategory = await context.SubCategories
                .FirstOrDefaultAsync(s => s.Slug == subCategorySlug);

            if (subCategory is null)
            {
                logger.LogWarning(
                    "SubCategory '{Slug}' not found. Skipping course '{Title}'.",
                    subCategorySlug,
                    c.GetProperty("title").GetString());
                continue;
            }

            // 3.  price
            var price = c.GetProperty("price").GetDecimal();

            decimal? discountPrice = null;
            if (c.TryGetProperty("discountPrice", out var dp) &&
                dp.ValueKind != JsonValueKind.Null)
                discountPrice = dp.GetDecimal();

            // 4. read arrays (requirements & whatYouLearn)
            var requirements = c.GetProperty("requirements")
                .EnumerateArray()
                .Select(r => r.GetString()!)
                .ToList();

            var whatYouLearn = c.GetProperty("whatYouLearn")
                .EnumerateArray()
                .Select(w => w.GetString()!)
                .ToList();

            // 5. Parse الـ level
            var levelStr = c.GetProperty("level").GetString()!;
            var level = Enum.Parse<CourseLevel>(levelStr);

            // 6. Create Course
            var course = new Course
            {
                Title = c.GetProperty("title").GetString()!,
                Description = c.GetProperty("description").GetString()!,
                ShortDescription = c.TryGetProperty("shortDescription", out var sd)
                                    ? sd.GetString() : null,
                Price = price,
                DiscountPrice = discountPrice,
                Level = level,
                Status = CourseStatus.Published,
                Language = c.GetProperty("language").GetString()!,
                ThumbnailUrl = c.TryGetProperty("thumbnailUrl", out var tu)
                ? tu.GetString()
                : null,
                SubCategoryId = subCategory.Id,
                InstructorId = instructor.Id,
                Requirements = JsonSerializer.Serialize(requirements),
                WhatYouLearn = JsonSerializer.Serialize(whatYouLearn),
            };

            // 7. read Sections
            var sectionOrder = 1;
            foreach (var s in c.GetProperty("sections").EnumerateArray())
            {
                var section = new Section
                {
                    Title = s.GetProperty("title").GetString()!,
                    Order = sectionOrder++,
                };

                // 8. read Lessons
                var lessonOrder = 1;
                foreach (var l in s.GetProperty("lessons").EnumerateArray())
                {
                    var typeStr = l.GetProperty("type").GetString()!;
                    var type = Enum.Parse<LessonType>(typeStr);

                    section.Lessons.Add(new Lesson
                    {
                        Title = l.GetProperty("title").GetString()!,
                        Description = l.TryGetProperty("description", out var desc)
                                            ? desc.GetString() : null,

                        VideoUrl = l.TryGetProperty("videoUrl", out var vu) &&
                                            vu.ValueKind != JsonValueKind.Null
                                            ? vu.GetString()
                                            : null,
                        DurationInSeconds = l.GetProperty("durationInSeconds").GetInt32(),
                        Type = type,
                        IsFreePreview = l.GetProperty("isFreePreview").GetBoolean(),
                        Order = lessonOrder++,
                    });

                    totalLessons++;
                }

                course.Sections.Add(section);
                totalSections++;
            }

            context.Courses.Add(course);
            totalCourses++;
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Courses seeded: {Courses} courses, {Sections} sections, {Lessons} lessons.",
            totalCourses, totalSections, totalLessons);
    }

    private static async Task SeedPlatformSettingsAsync(
        AppDbContext context, ILogger logger)
    {
        if (await context.PlatformSettings.AnyAsync()) return;

        var jsonPath = Path.Combine(
            AppContext.BaseDirectory, "SeedData", "settings.json");

        if (!File.Exists(jsonPath)) return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var document = JsonDocument.Parse(json);
        var settings = document.RootElement.GetProperty("settings");

        foreach (var s in settings.EnumerateArray())
        {
            context.PlatformSettings.Add(new PlatformSetting
            {
                Key = s.GetProperty("key").GetString()!,
                Value = s.GetProperty("value").GetString()!,
                Group = s.GetProperty("group").GetString()
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Platform settings seeded.");
    }
}