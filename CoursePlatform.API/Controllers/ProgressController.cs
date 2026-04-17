using CoursePlatform.Application.Features.Progress.Commands.MarkLessonComplete;
using CoursePlatform.Application.Features.Progress.Commands.UpdateWatchTime;
using CoursePlatform.Application.Features.Progress.DTOs;
using CoursePlatform.Application.Features.Progress.Queries.GetCourseProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly ISender _sender;

    public ProgressController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get full progress breakdown for a course.
    /// Shows completed/total lessons per section.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CourseProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CourseProgressDto>> GetProgress(
        int courseId, CancellationToken ct)
        => Ok(await _sender.Send(new GetCourseProgressQuery(courseId), ct));

    /// <summary>
    /// Mark a lesson as complete.
    /// Idempotent — safe to call multiple times.
    /// </summary>
    [HttpPost("lessons/{lessonId:int}/complete")]
    [ProducesResponseType(typeof(MarkLessonCompleteResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarkLessonCompleteResult>> MarkComplete(
        int courseId, int lessonId, CancellationToken ct)
        => Ok(await _sender.Send(
            new MarkLessonCompleteCommand(courseId, lessonId), ct));

    /// <summary>
    /// Update video watch time. Auto-completes lesson when >= 80% watched.
    /// Frontend should call this every 30 seconds and on pause/end.
    /// </summary>
    [HttpPost("lessons/{lessonId:int}/watch-time")]
    [ProducesResponseType(typeof(UpdateWatchTimeResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateWatchTimeResult>> UpdateWatchTime(
        int courseId, int lessonId,
        [FromBody] UpdateWatchTimeRequest request,
        CancellationToken ct)
    {
        var command = new UpdateWatchTimeCommand(
            courseId, lessonId,
            request.WatchedSeconds,
            request.TotalSeconds);

        return Ok(await _sender.Send(command, ct));
    }

    public record UpdateWatchTimeRequest(
        int WatchedSeconds,
        int TotalSeconds);
}