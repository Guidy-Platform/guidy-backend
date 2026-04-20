namespace CoursePlatform.Application.Features.InstructorDashboard.DTOs;

public class DashboardSummaryDto
{
    // Courses
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int DraftCourses { get; set; }
    public int PendingCourses { get; set; }

    // Students
    public int TotalStudents { get; set; }
    public int NewStudentsThisMonth { get; set; }

    // Revenue
    public decimal TotalRevenue { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
    public double RevenueGrowthPercent { get; set; }

    // Ratings
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}