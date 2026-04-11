using CoursePlatform.Application.Features.UserProfile.Commands.DeleteAvatar;
using CoursePlatform.Application.Features.UserProfile.Commands.UpdateProfile;
using CoursePlatform.Application.Features.UserProfile.Commands.UploadAvatar;
using CoursePlatform.Application.Features.UserProfile.DTOs;
using CoursePlatform.Application.Features.UserProfile.Queries.GetInstructorProfile;
using CoursePlatform.Application.Features.UserProfile.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
        => _sender = sender;

    /// <summary>Get current user's profile.</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyProfileQuery(), ct));

    /// <summary>Update profile info (name + bio).</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    /// <summary>Upload profile picture. Max 5MB. jpg/jpeg/png/webp only.</summary>
    [HttpPost("me/avatar")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileDto>> UploadAvatar(
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var command = new UploadAvatarCommand(
            FileStream: file.OpenReadStream(),
            FileName: file.FileName,
            FileSize: file.Length,
            ContentType: file.ContentType
        );

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Delete profile picture.</summary>
    [HttpDelete("me/avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAvatar(CancellationToken ct)
    {
        await _sender.Send(new DeleteAvatarCommand(), ct);
        return Ok(new { message = "Avatar deleted successfully." });
    }
    

    [HttpGet("instructor/{username}")]   // ← string مش guid
    [AllowAnonymous]
    [ProducesResponseType(typeof(InstructorProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorProfileDto>> GetInstructorProfile(
        string username,                 // ← string مش Guid
        CancellationToken ct)
        => Ok(await _sender.Send(new GetInstructorProfileQuery(username), ct));
}