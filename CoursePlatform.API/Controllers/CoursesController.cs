using CoursePlatform.Application.Common.Models;
using CoursePlatform.Application.Features.Courses.Commands.ApproveCourse;
using CoursePlatform.Application.Features.Courses.Commands.ArchiveCourse;
using CoursePlatform.Application.Features.Courses.Commands.CreateCourse;
using CoursePlatform.Application.Features.Courses.Commands.DeleteCourse;
using CoursePlatform.Application.Features.Courses.Commands.RejectCourse;
using CoursePlatform.Application.Features.Courses.Commands.SubmitCourseForReview;
using CoursePlatform.Application.Features.Courses.Commands.UnarchiveCourse;
using CoursePlatform.Application.Features.Courses.Commands.UpdateCourse;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Courses.Queries.GetCourseById;
using CoursePlatform.Application.Features.Courses.Queries.GetMyCourses;
using CoursePlatform.Application.Features.Courses.Queries.GetPendingCourses;
using CoursePlatform.Application.Features.Courses.Queries.GetPublishedCourses;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ISender _sender;

    public CoursesController(ISender sender)
        => _sender = sender;

    // ─── Public ───────────────────────────────────────────────────────────

    /// <summary>Get published courses with filters and pagination.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<CourseSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagination<CourseSummaryDto>>> GetPublished(
        [FromQuery] CourseQueryParams queryParams,
        CancellationToken ct)
        => Ok(await _sender.Send(new GetPublishedCoursesQuery(queryParams), ct));

    /// <summary>Get course details by ID.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetById(
        int id, CancellationToken ct)
        => Ok(await _sender.Send(new GetCourseByIdQuery(id), ct));

    // ─── Instructor ───────────────────────────────────────────────────────

    /// <summary>Get my courses (all statuses).</summary>
    [HttpGet("my")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> GetMyCourses(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyCoursesQuery(), ct));

    /// <summary>Create a new course (Draft).</summary>
    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> Create(
        [FromBody] CreateCourseCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update course info.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> Update(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCourseCommand(
            id, request.Title, request.Description,
            request.ShortDescription, request.Price,
            request.DiscountPrice, request.Level,
            request.Language, request.SubCategoryId,
            request.Requirements, request.WhatYouLearn);

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Delete course (Draft/Rejected only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _sender.Send(new DeleteCourseCommand(id), ct);
        return NoContent();
    }

    /// <summary>Submit course for admin review.</summary>
    [HttpPost("{id:int}/submit")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(int id, CancellationToken ct)
    {
        await _sender.Send(new SubmitCourseForReviewCommand(id), ct);
        return Ok(new { message = "Course submitted for review." });
    }

    /// <summary>Archive a published course.</summary>
    [HttpPut("{id:int}/archive")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        await _sender.Send(new ArchiveCourseCommand(id), ct);
        return Ok(new { message = "Course archived successfully." });
    }

    // ─── Admin ────────────────────────────────────────────────────────────

    /// <summary>Get all courses pending review.</summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummaryDto>>> GetPending(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetPendingCoursesQuery(), ct));

    /// <summary>Approve course → Published.</summary>
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        await _sender.Send(new ApproveCourseCommand(id), ct);
        return Ok(new { message = "Course approved and published." });
    }

    /// <summary>Reject course with reason.</summary>
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectCourseRequest request,
        CancellationToken ct)
    {
        await _sender.Send(new RejectCourseCommand(id, request.Reason), ct);
        return Ok(new { message = "Course rejected." });
    }

    [HttpPut("{id:int}/unarchive")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unarchive(int id, CancellationToken ct)
    {
        await _sender.Send(new UnarchiveCourseCommand(id), ct);
        return Ok(new
        {
            message = "Course unarchived. It is now in Draft status. " +
                      "Submit it for review when ready to publish again."
        });
    }

}

// ─── Request Models ────────────────────────────────────────────────────────
public record UpdateCourseRequest(
    string Title,
    string Description,
    string? ShortDescription,
    decimal Price,
    decimal? DiscountPrice,
    CourseLevel Level,
    string Language,
    int SubCategoryId,
    List<string> Requirements,
    List<string> WhatYouLearn);

public record RejectCourseRequest(string Reason);