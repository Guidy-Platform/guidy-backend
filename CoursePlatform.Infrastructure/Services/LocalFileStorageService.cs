using CoursePlatform.Application.Contracts.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace CoursePlatform.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _wwwrootPath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IWebHostEnvironment env,
        ILogger<LocalFileStorageService> logger)
    {
        // الملفات في wwwroot/uploads
        _wwwrootPath = env.WebRootPath
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        _logger = logger;
    }

    public async Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct = default)
    {
        var folderPath = Path.Combine(_wwwrootPath, "uploads", folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        await using var fs = new FileStream(
            filePath, FileMode.Create, FileAccess.Write,
            FileShare.None, bufferSize: 81920, useAsync: true);

        fileStream.Position = 0;  // تأكد من البداية
        await fileStream.CopyToAsync(fs, ct);

        _logger.LogInformation("File saved: uploads/{Folder}/{FileName}",
            folder, fileName);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.CompletedTask;

            // /uploads/resources/file.pdf → wwwroot/uploads/resources/file.pdf
            var relativePath = fileUrl.TrimStart('/')
                                      .Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_wwwrootPath, relativePath);
            var normalized = Path.GetFullPath(fullPath);

            // Security check — مش نحذف files خارج الـ wwwroot
            if (!normalized.StartsWith(
                    Path.GetFullPath(_wwwrootPath),
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Attempted path traversal attack: {Url}", fileUrl);
                return Task.CompletedTask;
            }

            if (File.Exists(normalized))
            {
                File.Delete(normalized);
                _logger.LogInformation("File deleted: {Path}", normalized);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file: {Url}", fileUrl);
        }

        return Task.CompletedTask;
    }

    public string GetFullUrl(string relativeUrl, string baseUrl)
        => $"{baseUrl.TrimEnd('/')}{relativeUrl}";
}