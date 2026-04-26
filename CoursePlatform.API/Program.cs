// API/Program.cs
using CoursePlatform.API.Extensions;
using CoursePlatform.API.Middleware;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// ─── Kestrel for Container ────────────────────────────────────────

// ─── CORS ─────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        //if (builder.Environment.IsDevelopment())
        //{
        //    policy.AllowAnyOrigin()
        //          .AllowAnyMethod()
        //          .AllowAnyHeader();
        //}
        //else
        //{
        //    policy.WithOrigins(
        //            builder.Configuration
        //                   .GetSection("AllowedOrigins")
        //                   .Get<string[]>() ?? []
        //          )
        //          .AllowAnyMethod()
        //          .AllowAnyHeader()
        //          .AllowCredentials();
        //}

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

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();  //  stream seekable
    await next();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guidy API v1");
    c.RoutePrefix = "swagger";
});


app.UseStaticFiles();

app.UseCors("FrontendPolicy");   


app.UseMiddleware<ExceptionMiddleware>();



app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Health check for Azure
app.MapGet("/health", () => Results.Ok(new
{
    status  = "healthy",
    version = "1.0.0",
    time    = DateTime.UtcNow
}));
try
{
    await app.InitializeDatabaseAsync();
}
catch (Exception ex)
{
    Console.WriteLine("DB init failed: " + ex.Message);
}

app.Run();