namespace CoursePlatform.Application.Features.Resources.DTOs;

public class ResourceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public DateTime CreatedAt { get; set; }
}