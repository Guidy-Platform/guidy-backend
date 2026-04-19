using CoursePlatform.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using CoursePlatform.Application.Features.Notifications.Commands.MarkNotificationRead;
using CoursePlatform.Application.Features.Notifications.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Get my notifications with unread count.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetMyNotificationsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMyNotificationsResult>> GetMine(
        [FromQuery] bool? unreadOnly,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetMyNotificationsQuery(unreadOnly), ct));

    /// <summary>Mark a single notification as read.</summary>
    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(
        int id, CancellationToken ct)
    {
        await _sender.Send(new MarkNotificationReadCommand(id), ct);
        return Ok(new { message = "Notification marked as read." });
    }

    /// <summary>Mark all my notifications as read.</summary>
    [HttpPatch("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var count = await _sender.Send(
            new MarkAllNotificationsReadCommand(), ct);
        return Ok(new { message = $"{count} notification(s) marked as read." });
    }
}