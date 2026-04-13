using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Entities;

public class Resource : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;  // original name with extension
    public string FileType { get; set; } = string.Empty;  // pdf, zip, mp4...
    public long FileSize { get; set; }                  // bytes
    public int LessonId { get; set; }

    public Lesson Lesson { get; set; } = null!;

    // Computed 
    public string FileSizeFormatted => FormatSize(FileSize);

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _ => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
}