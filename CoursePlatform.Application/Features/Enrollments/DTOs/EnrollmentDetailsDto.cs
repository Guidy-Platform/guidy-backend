namespace CoursePlatform.Application.Features.Enrollments.DTOs;

public class EnrollmentDetailsDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public double ProgressPercent { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public bool IsCompleted { get; set; }
}