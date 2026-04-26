using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.DTOs;
using CoursePlatform.Application.Features.Search.Queries.GetPopularCourses;
using CoursePlatform.Application.Features.Search.Queries.GetRelatedCourses;
using CoursePlatform.Application.Features.Search.Queries.GetSearchSuggestions;
using CoursePlatform.Application.Features.Search.Queries.GetTrendingCourses;
using CoursePlatform.Application.Features.Search.Queries.SearchCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/search")]
[AllowAnonymous]
public class SearchController : ControllerBase
{
    private readonly ISender _sender;

    public SearchController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get search suggestions (autocomplete) — min 2 chars.
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(IReadOnlyList<SearchSuggestionDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SearchSuggestionDto>>> Suggestions(
        [FromQuery] string q,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetSearchSuggestionsQuery(q), ct));

    /// <summary>
    /// Get related courses for a specific course.
    /// </summary>
    [HttpGet("related/{courseId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseSummaryDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> Related(
        int courseId, CancellationToken ct)
        => Ok(await _sender.Send(
            new GetRelatedCoursesQuery(courseId), ct));

    /// <summary>
    /// Get most popular courses by enrollment count.
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseSummaryDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> Popular(
        [FromQuery] int take = 10,
        CancellationToken ct = default)
        => Ok(await _sender.Send(
            new GetPopularCoursesQuery(take), ct));

    /// <summary>
    /// Get trending courses (last 30 days by enrollment).
    /// </summary>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseSummaryDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> Trending(
        [FromQuery] int take = 10,
        CancellationToken ct = default)
        => Ok(await _sender.Send(
            new GetTrendingCoursesQuery(take), ct));
}