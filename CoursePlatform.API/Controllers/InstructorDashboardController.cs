using CoursePlatform.Application.Features.InstructorDashboard.DTOs;
using CoursePlatform.Application.Features.InstructorDashboard.Queries.GetDashboardSummary;
using CoursePlatform.Application.Features.InstructorDashboard.Queries.GetEnrollmentStats;
using CoursePlatform.Application.Features.InstructorDashboard.Queries.GetRevenueStats;
using CoursePlatform.Application.Features.InstructorDashboard.Queries.GetTopCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/instructor/dashboard")]
[Authorize(Roles = "Instructor")]
public class InstructorDashboardController : ControllerBase
{
    private readonly ISender _sender;

    public InstructorDashboardController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get overall dashboard summary:
    /// courses, students, revenue, ratings.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetDashboardSummaryQuery(), ct));

    /// <summary>
    /// Get monthly revenue for the last N months.
    /// Default: last 12 months.
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RevenueStatsDto>> GetRevenue(
        [FromQuery] int months = 12,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetRevenueStatsQuery(months), ct));

    /// <summary>
    /// Get monthly enrollments for the last N months.
    /// Default: last 12 months.
    /// </summary>
    [HttpGet("enrollments")]
    [ProducesResponseType(typeof(EnrollmentStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EnrollmentStatsDto>> GetEnrollments(
        [FromQuery] int months = 12,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetEnrollmentStatsQuery(months), ct));

    /// <summary>
    /// Get top performing courses.
    /// sortBy: enrollments | revenue | rating
    /// </summary>
    [HttpGet("top-courses")]
    [ProducesResponseType(typeof(IReadOnlyList<TopCourseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TopCourseDto>>> GetTopCourses(
        [FromQuery] int top = 5,
        [FromQuery] string sortBy = "enrollments",
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetTopCoursesQuery(top, sortBy), ct));
}