using CoursePlatform.Application.Features.Certificates.Commands.IssueCertificate;
using CoursePlatform.Application.Features.Certificates.DTOs;
using CoursePlatform.Application.Features.Certificates.Queries.GetCertificateById;
using CoursePlatform.Application.Features.Certificates.Queries.GetMyCertificates;
using CoursePlatform.Application.Features.Certificates.Queries.VerifyCertificate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/certificates")]
public class CertificatesController : ControllerBase
{
    private readonly ISender _sender;

    public CertificatesController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Issue a certificate after completing a course (100%).
    /// Idempotent — safe to call multiple times.
    /// </summary>
    [HttpPost("courses/{courseId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CertificateDto>> Issue(
        int courseId, CancellationToken ct)
    {
        var result = await _sender.Send(
            new IssueCertificateCommand(courseId), ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Get all my certificates.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<CertificateDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CertificateDto>>> GetMine(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyCertificatesQuery(), ct));

    /// <summary>
    /// Download certificate as PDF.
    /// </summary>
    [HttpGet("{certificateId:int}/download")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        int certificateId, CancellationToken ct)
    {
        var pdfBytes = await _sender.Send(
            new GetCertificateByIdQuery(certificateId), ct);

        return File(
            pdfBytes,
            "application/pdf",
            $"certificate-{certificateId}.pdf");
    }

    /// <summary>
    /// Verify a certificate by its unique code.
    /// Public endpoint — no auth required.
    /// </summary>
    [HttpGet("verify/{verifyCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CertificateVerifyDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CertificateVerifyDto>> Verify(
        string verifyCode, CancellationToken ct)
        => Ok(await _sender.Send(
            new VerifyCertificateQuery(verifyCode), ct));
}