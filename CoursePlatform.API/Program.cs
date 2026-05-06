using CoursePlatform.API.Extensions;
using CoursePlatform.API.Middleware;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// ─── Kestrel for Container ────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// ─── CORS ─────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// File Limitation
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104_857_600;  // 100MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104_857_600;  // 100MB
});

var app = builder.Build();

// ─── Kestrel binding for container ────────────────────────────────
app.Urls.Add($"http://0.0.0.0:{port}");

// ─── Global exception middleware (always first) ───────────────────
app.UseMiddleware<ExceptionMiddleware>();

// ─── Enable buffering BEFORE other middleware ─────────────────────
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// ─── Static files ─────────────────────────────────────────────────
app.UseStaticFiles();

// ─── Swagger ──────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guidy API v1");
    c.RoutePrefix = "swagger";
});

// ─── CORS ─────────────────────────────────────────────────────────
app.UseCors("FrontendPolicy");

// ─── Auth ─────────────────────────────────────────────────────────
app.UseAuthentication();
app.UseAuthorization();

// ─── Controllers ──────────────────────────────────────────────────
app.MapControllers();

// ─── Health check ─────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    version = "1.0.0",
    time = DateTime.UtcNow
}));

// ─── DB Init ──────────────────────────────────────────────────────
await app.InitializeDatabaseAsync();

app.Run();