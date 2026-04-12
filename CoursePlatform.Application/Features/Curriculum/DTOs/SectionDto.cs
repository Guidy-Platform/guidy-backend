namespace CoursePlatform.Application.Features.Curriculum.DTOs;

public class SectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int LessonCount { get; set; }
    public int TotalSeconds { get; set; }
    public string TotalDuration => FormatDuration(TotalSeconds);
    public IList<LessonDto> Lessons { get; set; } = [];

    private static string FormatDuration(int seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.Hours > 0
            ? $"{ts.Hours}h {ts.Minutes}m"
            : $"{ts.Minutes}m";
    }
}