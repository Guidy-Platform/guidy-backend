using CoursePlatform.Application.Features.Courses.Queries.GetCoursesFilter;
using CoursePlatform.Application.Features.Courses.Queries.GetMyCourses;
using CoursePlatform.Application.Features.Reviews.DTOs;
using CoursePlatform.Application.Features.Reviews.Queries.GetAdminReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminReviewsController(ISender sender)
        => _sender = sender;

    /// <summary>Get all reviews — filter by courseId, rating.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(AdminReviewsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminReviewsDto>> GetAll(
        [FromQuery] int? courseId,
        [FromQuery] int? rating,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetAdminReviewsQuery(courseId, rating), ct));

    /// <summary>
    /// Get courses list for filter dropdown.
    /// Use ?search=python to search by title.
    /// </summary>
    [HttpGet("courses-filter")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseFilterItemDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseFilterItemDto>>> GetCoursesFilter(
        [FromQuery] string? search,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetCoursesFilterQuery(search), ct));
}