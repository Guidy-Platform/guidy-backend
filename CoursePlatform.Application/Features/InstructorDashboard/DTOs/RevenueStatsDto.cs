namespace CoursePlatform.Application.Features.InstructorDashboard.DTOs;

public class RevenueStatsDto
{
    public IList<MonthlyStatDto> MonthlyRevenue { get; set; } = [];
    public decimal TotalRevenue { get; set; }
    public decimal AveragePerMonth { get; set; }
    public decimal BestMonthRevenue { get; set; }
    public string BestMonth { get; set; } = string.Empty;
}

public class MonthlyStatDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;  // "Jan 2026"
    public decimal Amount { get; set; }
    public int Count { get; set; }  // عدد الـ enrollments / orders
}