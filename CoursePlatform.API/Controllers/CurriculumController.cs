using CoursePlatform.Application.Features.Curriculum.Commands.AddLesson;
using CoursePlatform.Application.Features.Curriculum.Commands.AddSection;
using CoursePlatform.Application.Features.Curriculum.Commands.DeleteLesson;
using CoursePlatform.Application.Features.Curriculum.Commands.DeleteSection;
using CoursePlatform.Application.Features.Curriculum.Commands.ReorderLessons;
using CoursePlatform.Application.Features.Curriculum.Commands.ReorderSections;
using CoursePlatform.Application.Features.Curriculum.Commands.ToggleFreePreview;
using CoursePlatform.Application.Features.Curriculum.Commands.UpdateLesson;
using CoursePlatform.Application.Features.Curriculum.Commands.UpdateSection;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Queries.GetCourseCurriculum;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/curriculum")]
public class CurriculumController : ControllerBase
{
    private readonly ISender _sender;

    public CurriculumController(ISender sender)
        => _sender = sender;

    // ─── Public / Student ─────────────────────────────────────────────────

    /// <summary>
    /// Get course curriculum.
    /// Public: free preview lessons only.
    /// Instructor/Admin: all lessons.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseCurriculumDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CourseCurriculumDto>> GetCurriculum(
        int courseId, CancellationToken ct)
        => Ok(await _sender.Send(
            new GetCourseCurriculumQuery(courseId), ct));

    // ─── Sections ─────────────────────────────────────────────────────────

    /// <summary>Add a new section to the course.</summary>
    [HttpPost("sections")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(SectionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SectionDto>> AddSection(
        int courseId,
        [FromBody] AddSectionRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AddSectionCommand(courseId, request.Title), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Update section title.</summary>
    [HttpPut("sections/{sectionId:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(SectionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SectionDto>> UpdateSection(
        int courseId, int sectionId,
        [FromBody] AddSectionRequest request,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new UpdateSectionCommand(sectionId, courseId, request.Title), ct));

    /// <summary>Delete a section (and all its lessons).</summary>
    [HttpDelete("sections/{sectionId:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSection(
        int courseId, int sectionId, CancellationToken ct)
    {
        await _sender.Send(
            new DeleteSectionCommand(sectionId, courseId), ct);
        return NoContent();
    }

    /// <summary>Reorder sections (drag and drop).</summary>
    [HttpPut("sections/reorder")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReorderSections(
        int courseId,
        [FromBody] ReorderSectionsRequest request,
        CancellationToken ct)
    {
        await _sender.Send(
            new ReorderSectionsCommand(courseId, request.Items), ct);
        return Ok(new { message = "Sections reordered successfully." });
    }

    // ─── Lessons ──────────────────────────────────────────────────────────

    /// <summary>Add a lesson to a section.</summary>
    [HttpPost("sections/{sectionId:int}/lessons")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<LessonDto>> AddLesson(
        int courseId, int sectionId,
        [FromBody] AddLessonRequest request,
        CancellationToken ct)
    {
        var command = new AddLessonCommand(
            courseId, sectionId,
            request.Title, request.Description,
            request.VideoUrl, request.DurationInSeconds,
            request.Type, request.IsFreePreview);

        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Update lesson details.</summary>
    [HttpPut("sections/{sectionId:int}/lessons/{lessonId:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LessonDto>> UpdateLesson(
        int courseId, int sectionId, int lessonId,
        [FromBody] AddLessonRequest request,
        CancellationToken ct)
    {
        var command = new UpdateLessonCommand(
            lessonId, sectionId, courseId,
            request.Title, request.Description,
            request.VideoUrl, request.DurationInSeconds,
            request.Type, request.IsFreePreview);

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Delete a lesson.</summary>
    [HttpDelete("sections/{sectionId:int}/lessons/{lessonId:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLesson(
        int courseId, int sectionId, int lessonId,
        CancellationToken ct)
    {
        await _sender.Send(
            new DeleteLessonCommand(lessonId, sectionId, courseId), ct);
        return NoContent();
    }

    /// <summary>Reorder lessons within a section.</summary>
    [HttpPut("sections/{sectionId:int}/lessons/reorder")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReorderLessons(
        int courseId, int sectionId,
        [FromBody] ReorderLessonsRequest request,
        CancellationToken ct)
    {
        await _sender.Send(
            new ReorderLessonsCommand(courseId, sectionId, request.Items), ct);
        return Ok(new { message = "Lessons reordered successfully." });
    }

    /// <summary>Toggle free preview for a lesson.</summary>
    [HttpPatch("sections/{sectionId:int}/lessons/{lessonId:int}/toggle-preview")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleFreePreview(
        int courseId, int sectionId, int lessonId,
        CancellationToken ct)
    {
        var isFreePreview = await _sender.Send(
            new ToggleFreePreviewCommand(lessonId, sectionId, courseId), ct);

        return Ok(new
        {
            lessonId,
            isFreePreview,
            message = isFreePreview
                ? "Lesson is now free preview."
                : "Lesson is now paid only."
        });
    }
}

// ─── Request Models ────────────────────────────────────────────────────────
public record AddSectionRequest(string Title);

public record AddLessonRequest(
    string Title,
    string? Description,
    string? VideoUrl,
    int DurationInSeconds,
    LessonType Type,
    bool IsFreePreview);

public record ReorderSectionsRequest(IList<SectionOrderItem> Items);
public record ReorderLessonsRequest(IList<LessonOrderItem> Items);