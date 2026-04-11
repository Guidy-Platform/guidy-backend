using CoursePlatform.Application.Contracts.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoursePlatform.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsRoot;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IHostEnvironment env,
        ILogger<LocalFileStorageService> logger)
    {
        // الملفات بتتحفظ في wwwroot/uploads
        _uploadsRoot = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        _logger = logger;
    }

    public async Task<string> SaveAsync(
        Stream fileStream, string fileName,
        string folder, CancellationToken ct = default)
    {
        // إنشاء الـ folder لو مش موجود
        var folderPath = Path.Combine(_uploadsRoot, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        await using var fs = new FileStream(
            filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fs, ct);

        _logger.LogInformation("File saved: {Path}", filePath);

        // رجعي الـ relative URL للـ client
        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl)) return Task.CompletedTask;

            // حول الـ URL لـ physical path
            // /uploads/avatars/file.jpg → wwwroot/uploads/avatars/file.jpg
            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(
                _uploadsRoot, "..", relativePath);
            var normalizedPath = Path.GetFullPath(fullPath);

            if (File.Exists(normalizedPath))
            {
                File.Delete(normalizedPath);
                _logger.LogInformation("File deleted: {Path}", normalizedPath);
            }
        }
        catch (Exception ex)
        {
            // مش بنرمي error لو الحذف فشل — مش critical
            _logger.LogWarning(ex, "Failed to delete file: {Url}", fileUrl);
        }

        return Task.CompletedTask;
    }
}