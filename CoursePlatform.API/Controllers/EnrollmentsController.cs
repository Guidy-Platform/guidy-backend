using CoursePlatform.Application.Features.Enrollments.DTOs;
using CoursePlatform.Application.Features.Enrollments.Queries.CheckEnrollment;
using CoursePlatform.Application.Features.Enrollments.Queries.GetEnrolledCourseDetails;
using CoursePlatform.Application.Features.Enrollments.Queries.GetMyEnrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly ISender _sender;

    public EnrollmentsController(ISender sender)
        => _sender = sender;

    /// <summary>Get all my enrolled courses with progress summary.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EnrollmentDto>>> GetMyEnrollments(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyEnrollmentsQuery(), ct));

    /// <summary>Check if I am enrolled in a specific course.</summary>
    [HttpGet("{courseId:int}/check")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckEnrollment(
        int courseId, CancellationToken ct)
        => Ok(await _sender.Send(new CheckEnrollmentQuery(courseId), ct));

    /// <summary>
    /// Get enrolled course details with progress summary.
    /// </summary>
    [HttpGet("{courseId:int}")]
    [ProducesResponseType(typeof(EnrollmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EnrollmentDetailsDto>> GetEnrolledCourseDetails(
        int courseId, CancellationToken ct)
        => Ok(await _sender.Send(
            new GetEnrolledCourseDetailsQuery(courseId), ct));
}