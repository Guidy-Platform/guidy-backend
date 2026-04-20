// API/Extensions/ApplicationServiceExtensions.cs
using CoursePlatform.Application;
using CoursePlatform.Infrastructure;

namespace CoursePlatform.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration config , IWebHostEnvironment env)
    {
        services.AddApplicationServices();          // Application DI
        services.AddInfrastructureServices(config, env); // Infrastructure DI
        return services;
    }
}