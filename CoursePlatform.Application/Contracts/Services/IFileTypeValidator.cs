namespace CoursePlatform.Application.Contracts.Services;

public interface IFileTypeValidator
{
    bool IsAllowedExtension(string extension);
    bool IsAllowedContentType(string extension, string contentType);
    Task<bool> IsValidFileSignatureAsync(Stream stream, string extension,
        CancellationToken ct = default);
    string[] GetAllowedExtensions();
    string GetFileType(string extension);
}