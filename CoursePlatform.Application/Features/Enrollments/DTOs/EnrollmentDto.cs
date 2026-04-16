namespace CoursePlatform.Application.Features.Enrollments.DTOs;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public DateTime EnrolledAt { get; set; }
}