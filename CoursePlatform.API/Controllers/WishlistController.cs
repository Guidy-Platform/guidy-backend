using CoursePlatform.Application.Features.Wishlist.Commands.AddToWishlist;
using CoursePlatform.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using CoursePlatform.Application.Features.Wishlist.DTOs;
using CoursePlatform.Application.Features.Wishlist.Queries.GetMyWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ISender _sender;

    public WishlistController(ISender sender)
        => _sender = sender;

    /// <summary>Get my wishlist.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WishlistItemDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WishlistItemDto>>> GetMyWishlist(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyWishlistQuery(), ct));

    /// <summary>Add a course to wishlist.</summary>
    [HttpPost("{courseId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(int courseId, CancellationToken ct)
    {
        await _sender.Send(new AddToWishlistCommand(courseId), ct);
        return Ok(new { message = "Course added to wishlist." });
    }

    /// <summary>Remove a course from wishlist.</summary>
    [HttpDelete("{courseId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(int courseId, CancellationToken ct)
    {
        await _sender.Send(new RemoveFromWishlistCommand(courseId), ct);
        return Ok(new { message = "Course removed from wishlist." });
    }
}