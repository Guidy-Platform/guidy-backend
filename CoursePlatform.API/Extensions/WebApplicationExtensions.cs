using CoursePlatform.Infrastructure.Persistence;

namespace CoursePlatform.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await DbInitializer.InitializeAsync(app.Services);
    }
}