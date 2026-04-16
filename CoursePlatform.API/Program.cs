// API/Program.cs
using CoursePlatform.API.Extensions;
using CoursePlatform.API.Middleware;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
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
    context.Request.EnableBuffering();  // ← بيخلي الـ stream seekable
    await next();
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();




app.UseMiddleware<ExceptionMiddleware>();



app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();