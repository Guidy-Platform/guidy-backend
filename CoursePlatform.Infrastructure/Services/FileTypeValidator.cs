using CoursePlatform.Application.Contracts.Services;

namespace CoursePlatform.Infrastructure.Services;

public class FileTypeValidator : IFileTypeValidator
{
    private static readonly Dictionary<string, string[]> AllowedTypes = new()
    {
        [".pdf"] = ["application/pdf"],
        [".zip"] = ["application/zip", "application/x-zip-compressed"],
        [".docx"] = ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"],
        [".xlsx"] = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"],
        [".mp4"] = ["video/mp4"],
        [".png"] = ["image/png"],
        [".jpg"] = ["image/jpeg"],
        [".jpeg"] = ["image/jpeg"],
    };

    private static readonly Dictionary<string, byte[]> MagicBytes = new()
    {
        [".pdf"] = [0x25, 0x50, 0x44, 0x46],
        [".zip"] = [0x50, 0x4B, 0x03, 0x04],
        [".docx"] = [0x50, 0x4B, 0x03, 0x04],
        [".xlsx"] = [0x50, 0x4B, 0x03, 0x04],
        [".mp4"] = [0x00, 0x00, 0x00, 0x18],
        [".png"] = [0x89, 0x50, 0x4E, 0x47],
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
    };

    public bool IsAllowedExtension(string extension)
        => AllowedTypes.ContainsKey(extension.ToLowerInvariant());

    public bool IsAllowedContentType(string extension, string contentType)
    {
        var ext = extension.ToLowerInvariant();
        return AllowedTypes.TryGetValue(ext, out var types) &&
               types.Contains(contentType.ToLowerInvariant());
    }

    public async Task<bool> IsValidFileSignatureAsync(
        Stream stream, string extension,
        CancellationToken ct = default)
    {
        var ext = extension.ToLowerInvariant();

        if (!MagicBytes.TryGetValue(ext, out var expectedBytes))
            return true;

        var buffer = new byte[expectedBytes.Length];
        var originalPosition = stream.Position;

        try
        {
            stream.Position = 0;
            var bytesRead = await stream.ReadAsync(buffer, ct);

            return bytesRead >= expectedBytes.Length &&
                   buffer.SequenceEqual(expectedBytes);
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    public string[] GetAllowedExtensions()
        => [.. AllowedTypes.Keys];

    public string GetFileType(string extension)
        => extension.ToLowerInvariant().TrimStart('.');
}