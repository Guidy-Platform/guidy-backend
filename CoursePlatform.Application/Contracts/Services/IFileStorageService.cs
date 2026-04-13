namespace CoursePlatform.Application.Contracts.Services;

public interface IFileStorageService
{
    /// <summary>
    /// save relative URL
    /// </summary>
    Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct = default);

    /// <summary>
    /// don't throw exception if the file doesn't exist, just ignore
    /// </summary>
    Task DeleteAsync(
        string fileUrl,
        CancellationToken ct = default);

    /// <summary>
    /// full URL for client access, e.g. https://cdn.courseplatform.com/files/abc123.pdf
    /// </summary>
    string GetFullUrl(string relativeUrl, string baseUrl);
}