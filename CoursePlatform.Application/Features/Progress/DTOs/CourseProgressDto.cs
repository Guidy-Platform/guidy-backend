namespace CoursePlatform.Application.Features.Progress.DTOs;

public class CourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public double ProgressPercent { get; set; }
    public bool IsCompleted { get; set; }
    public List<SectionProgressDto> Sections { get; set; } = [];
}

public class SectionProgressDto
{
    public int SectionId { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public List<LessonProgressDto> Lessons { get; set; } = [];
}