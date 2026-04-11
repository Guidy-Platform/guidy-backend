namespace CoursePlatform.Application.Contracts.Services;

public interface IFileStorageService
{
    /// <summary>
    /// يحفظ الـ file ويرجع الـ relative URL
    /// </summary>
    Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct = default);

    /// <summary>
    /// يحذف الـ file — مش بيرمي error لو مش موجود
    /// </summary>
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
}