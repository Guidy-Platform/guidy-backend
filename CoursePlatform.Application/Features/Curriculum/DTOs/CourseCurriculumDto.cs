namespace CoursePlatform.Application.Features.Curriculum.DTOs;

public class CourseCurriculumDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalSections { get; set; }
    public int TotalLessons { get; set; }
    public int TotalSeconds { get; set; }
    public string TotalDuration => FormatDuration(TotalSeconds);
    public int FreePreviewCount { get; set; }
    public IList<SectionDto> Sections { get; set; } = [];

    private static string FormatDuration(int seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.Hours > 0
            ? $"{ts.Hours}h {ts.Minutes}m"
            : $"{ts.Minutes}m";
    }
}