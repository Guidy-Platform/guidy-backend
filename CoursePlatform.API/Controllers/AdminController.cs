using CoursePlatform.Application.Features.Admin.Commands.BanUser;
using CoursePlatform.Application.Features.Admin.Commands.ChangeUserRole;
using CoursePlatform.Application.Features.Admin.Commands.UnbanUser;
using CoursePlatform.Application.Features.Admin.DTOs;
using CoursePlatform.Application.Features.Admin.Queries.GetAllCoursesAdmin;
using CoursePlatform.Application.Features.Admin.Queries.GetAllUsers;
using CoursePlatform.Application.Features.Admin.Queries.GetPlatformStats;
using CoursePlatform.Application.Features.Admin.Queries.GetUserById;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender)
        => _sender = sender;

    // ─── Platform Stats ───────────────────────────────────────────

    /// <summary>Get platform overview statistics.</summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(PlatformStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlatformStatsDto>> GetStats(
        [FromQuery] int months = 6,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetPlatformStatsQuery(months), ct));

    // ─── User Management ──────────────────────────────────────────

    /// <summary>Get all users with optional filters.</summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminUserDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminUserDto>>> GetUsers(
        [FromQuery] string? search = null,
        [FromQuery] bool? isBanned = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(
            new GetAllUsersQuery(search, isBanned, page, pageSize), ct));

    /// <summary>Get user details by ID.</summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserDto>> GetUser(
        Guid userId, CancellationToken ct)
        => Ok(await _sender.Send(new GetUserByIdQuery(userId), ct));

    /// <summary>Ban a user.</summary>
    [HttpPatch("users/{userId:guid}/ban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BanUser(
        Guid userId,
        [FromBody] BanUserRequest request,
        CancellationToken ct)
    {
        await _sender.Send(new BanUserCommand(userId, request.Reason), ct);
        return Ok(new { message = "User has been banned." });
    }

    /// <summary>Unban a user.</summary>
    [HttpPatch("users/{userId:guid}/unban")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnbanUser(
        Guid userId, CancellationToken ct)
    {
        await _sender.Send(new UnbanUserCommand(userId), ct);
        return Ok(new { message = "User has been unbanned." });
    }

    /// <summary>Change user role (Student ↔ Instructor).</summary>
    [HttpPatch("users/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(
        Guid userId,
        [FromBody] ChangeRoleRequest request,
        CancellationToken ct)
    {
        await _sender.Send(
            new ChangeUserRoleCommand(userId, request.NewRole), ct);
        return Ok(new { message = $"User role changed to {request.NewRole}." });
    }

    // ─── Content Moderation ───────────────────────────────────────

    /// <summary>Get all courses (any status, any instructor).</summary>
    [HttpGet("courses")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminCourseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminCourseDto>>> GetCourses(
        [FromQuery] CourseStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
        => Ok(await _sender.Send(
            new GetAllCoursesAdminQuery(status, search), ct));
}

// ─── Request Models ────────────────────────────────────────────────
public record BanUserRequest(string Reason);
public record ChangeRoleRequest(string NewRole);