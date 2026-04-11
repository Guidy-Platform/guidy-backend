// Application/DependencyInjection.cs
using CoursePlatform.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            // automatically register all IRequestHandler, INotificationHandler, etc. from the assembly
            cfg.RegisterServicesFromAssembly(assembly);
            // add pipeline behaviors in order .
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }); 

        // automatically register all AutoMapper profiles from the assembly
        services.AddAutoMapper(assembly); // scope is Singleton

        // automatically register all IValidator<T> from the assembly
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}