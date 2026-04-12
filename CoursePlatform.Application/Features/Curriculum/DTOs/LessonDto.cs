namespace CoursePlatform.Application.Features.Curriculum.DTOs;

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationInSeconds { get; set; }
    public string Duration => FormatDuration(DurationInSeconds);
    public int Order { get; set; }
    public bool IsFreePreview { get; set; }
    public string Type { get; set; } = string.Empty;
    public int ResourceCount { get; set; }

    private static string FormatDuration(int seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.Hours > 0
            ? $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"
            : $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }
}