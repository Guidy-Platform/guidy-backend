namespace CoursePlatform.Application.Features.InstructorDashboard.DTOs;

public class TopCourseDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Enrollments { get; set; }
    public decimal Revenue { get; set; }
    public double Rating { get; set; }
    public int Reviews { get; set; }
}