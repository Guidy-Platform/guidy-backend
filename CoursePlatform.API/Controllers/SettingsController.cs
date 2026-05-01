using CoursePlatform.Application.Features.Settings.Commands.UpdateSettings;
using CoursePlatform.Application.Features.Settings.DTOs;
using CoursePlatform.Application.Features.Settings.Queries.GetSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SettingsController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get platform settings.
    /// Public — Frontend uses this for branding, social links, etc.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PlatformSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlatformSettingsDto>> Get(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetSettingsQuery(), ct));

    /// <summary>
    /// Update platform settings. Admin only.
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PlatformSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlatformSettingsDto>> Update(
        [FromBody] PlatformSettingsDto dto,
        CancellationToken ct)
        => Ok(await _sender.Send(new UpdateSettingsCommand(dto), ct));
}