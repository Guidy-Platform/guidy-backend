using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence;
using CoursePlatform.Infrastructure.Persistence.Interceptors;
using CoursePlatform.Infrastructure.Persistence.Repositories;
using CoursePlatform.Infrastructure.Services;
using CoursePlatform.Infrastructure.Services.Consumers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

namespace CoursePlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // DbContext
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // For PostgreSQL Deployment:
        //services.AddDbContext<AppDbContext>(opts =>
        //         opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        // Identity — Guid PK
        services.AddIdentity<AppUser, IdentityRole<Guid>>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 8;
            opts.Password.RequireUppercase = true;
            opts.Password.RequireNonAlphanumeric = true;
            opts.Lockout.MaxFailedAccessAttempts = 5;
            opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero  // No tolerance — token expires exactly on time
            };
        });
        // Repository & UoW
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IAuthService, AuthService>();
        // Infrastructure/DependencyInjection.cs
        services.AddScoped<IOtpService, OtpService>();

        // RabbitMQ Email Consumer كـ BackgroundService
        services.AddHostedService<EmailConsumer>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // File handling
        services.AddScoped<IFileTypeValidator, FileTypeValidator>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        // Payment
        services.AddScoped<IPaymentService, StripePaymentService>();

        // Certificate generation
        services.AddScoped<ICertificateService, PdfCertificateService>();


        services.AddScoped<IUserRepository, UserRepository>();

        // Notification system
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHostedService<NotificationConsumer>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                config.GetConnectionString("Redis")!));
        services.AddScoped<ICacheService, RedisCacheService>();

        // RabbitMQ

        services.AddSingleton<IConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IConnection>>();
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = config["RabbitMQ:Host"] ?? "localhost",
                    UserName = config["RabbitMQ:Username"] ?? "guest",
                    Password = config["RabbitMQ:Password"] ?? "guest",
                    AutomaticRecoveryEnabled = true,   // ← بيعمل reconnect تلقائي
                };
                var conn = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                logger.LogInformation("RabbitMQ connected successfully.");
                return conn;
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    "RabbitMQ not available: {Message}", ex.Message);
                return null!;
            }
        });

        // Publisher → Transient (مش Scoped — عشان مش بيمسك state)
        services.AddTransient<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}