namespace CoursePlatform.Application.Features.UserProfile.DTOs;

public class InstructorProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int TotalCourses { get; set; }
    public int TotalStudents { get; set; }
    public double AverageRating { get; set; }
}