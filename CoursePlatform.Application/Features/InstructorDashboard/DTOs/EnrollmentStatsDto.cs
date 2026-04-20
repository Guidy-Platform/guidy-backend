namespace CoursePlatform.Application.Features.InstructorDashboard.DTOs;

public class EnrollmentStatsDto
{
    public IList<MonthlyStatDto> MonthlyEnrollments { get; set; } = [];
    public int TotalEnrollments { get; set; }
    public int ThisMonthCount { get; set; }
    public int LastMonthCount { get; set; }
    public double GrowthPercent { get; set; }
}