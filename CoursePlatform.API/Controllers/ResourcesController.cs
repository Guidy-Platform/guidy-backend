using CoursePlatform.Application.Features.Resources.Commands.DeleteResource;
using CoursePlatform.Application.Features.Resources.Commands.UploadResource;
using CoursePlatform.Application.Features.Resources.DTOs;
using CoursePlatform.Application.Features.Resources.Queries.GetLessonResources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/curriculum/sections/{sectionId:int}/lessons/{lessonId:int}/resources")]
public class ResourcesController : ControllerBase
{
    private readonly ISender _sender;

    public ResourcesController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get all resources for a lesson.
    /// Free preview lessons: public access.
    /// Paid lessons: enrolled students only.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ResourceDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResourceDto>>> GetResources(
        int courseId, int sectionId, int lessonId,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetLessonResourcesQuery(lessonId, sectionId, courseId), ct));

    /// <summary>
    /// Upload a resource file to a lesson.
    /// Allowed: PDF, ZIP, DOCX, XLSX, MP4, PNG, JPG/JPEG
    /// Max size: 100MB
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [RequestSizeLimit(104_857_600)]         // 100MB
    [RequestFormLimits(MultipartBodyLengthLimit = 104_857_600)]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResourceDto>> Upload(
        int courseId, int sectionId, int lessonId,
        [FromForm] UploadResourceRequest request,
        CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var command = new UploadResourceCommand(
            LessonId: lessonId,
            SectionId: sectionId,
            CourseId: courseId,
            Title: request.Title,
            FileStream: request.File.OpenReadStream(),
            FileName: request.File.FileName,
            ContentType: request.File.ContentType,
            FileSize: request.File.Length
        );

        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Delete a resource and its file from storage.
    /// </summary>
    [HttpDelete("{resourceId:int}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int courseId, int sectionId, int lessonId,
        int resourceId, CancellationToken ct)
    {
        await _sender.Send(
            new DeleteResourceCommand(
                resourceId, lessonId, sectionId, courseId), ct);

        return NoContent();
    }
}

// ─── Request Model ─────────────────────────────────────────────────────────
public class UploadResourceRequest
{
    public string Title { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
}