// API/Extensions/ApplicationServiceExtensions.cs
using CoursePlatform.Application;
using CoursePlatform.Infrastructure;

namespace CoursePlatform.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddApplicationServices();          // Application DI
        services.AddInfrastructureServices(config); // Infrastructure DI
        return services;
    }
}