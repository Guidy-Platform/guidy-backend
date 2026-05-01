// API/Controllers/ContactUsController.cs
using CoursePlatform.Application.Features.ContactUs.Commands.ReplyToContactMessage;
using CoursePlatform.Application.Features.ContactUs.Commands.SendContactMessage;
using CoursePlatform.Application.Features.ContactUs.DTOs;
using CoursePlatform.Application.Features.ContactUs.Queries.GetAllContactMessages;
using CoursePlatform.Application.Features.ContactUs.Queries.GetContactMessageById;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactUsController : ControllerBase
{
    private readonly ISender _sender;

    public ContactUsController(ISender sender)
        => _sender = sender;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ContactMessageDto>> Send(
        [FromBody] SendContactMessageRequest request,
        CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new SendContactMessageCommand(
            request.FullName,
            request.Email,
            request.Subject,
            request.Message,
            ipAddress);  

        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }


    /// <summary>
    /// Get all contact messages. Admin only.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<ContactMessageDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContactMessageDto>>> GetAll(
        [FromQuery] ContactMessageStatus? status,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new GetAllContactMessagesQuery(status), ct));

    /// <summary>
    /// Get contact message by ID. Admin only.
    /// Auto-marks as Read.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContactMessageDto>> GetById(
        int id, CancellationToken ct)
        => Ok(await _sender.Send(new GetContactMessageByIdQuery(id), ct));

    /// <summary>
    /// Reply to a contact message. Admin only.
    /// Sends email to the user automatically.
    /// </summary>
    [HttpPost("{id:int}/reply")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactMessageDto>> Reply(
        int id,
        [FromBody] ReplyRequest request,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new ReplyToContactMessageCommand(id, request.Reply), ct));
}

public record ReplyRequest(string Reply);
public record SendContactMessageRequest(
    string FullName,
    string Email,
    string Subject,
    string Message);