// API/Controllers/CouponsController.cs
using CoursePlatform.Application.Features.Coupons.Commands.CreateCoupon;
using CoursePlatform.Application.Features.Coupons.Commands.DeleteCoupon;
using CoursePlatform.Application.Features.Coupons.Commands.ToggleCoupon;
using CoursePlatform.Application.Features.Coupons.Commands.UpdateCoupon;
using CoursePlatform.Application.Features.Coupons.DTOs;
using CoursePlatform.Application.Features.Coupons.Queries.GetAllCoupons;
using CoursePlatform.Application.Features.Coupons.Queries.GetCouponById;
using CoursePlatform.Application.Features.Coupons.Queries.ValidateCoupon;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
{
    private readonly ISender _sender;

    public CouponsController(ISender sender)
        => _sender = sender;

    // ─── Public ───────────────────────────────────────────────────

    /// <summary>
    /// Validate a coupon code and calculate discount.
    /// Used by students before checkout.
    /// </summary>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(typeof(CouponValidationDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CouponValidationDto>> Validate(
        [FromQuery] string code,
        [FromQuery] decimal orderAmount,
        CancellationToken ct)
        => Ok(await _sender.Send(
            new ValidateCouponQuery(code, orderAmount), ct));

    // ─── Admin ────────────────────────────────────────────────────

    /// <summary>Get all coupons. Optionally filter by active status.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<CouponDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CouponDto>>> GetAll(
        [FromQuery] bool? activeOnly,
        CancellationToken ct)
        => Ok(await _sender.Send(new GetAllCouponsQuery(activeOnly), ct));

    /// <summary>Get coupon by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDto>> GetById(
        int id, CancellationToken ct)
        => Ok(await _sender.Send(new GetCouponByIdQuery(id), ct));

    /// <summary>Create a new coupon.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CouponDto>> Create(
        [FromBody] CreateCouponRequest request,
        CancellationToken ct)
    {
        var command = new CreateCouponCommand(
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.UsageLimit,
            request.ExpiresAt);

        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Update coupon details.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDto>> Update(
        int id,
        [FromBody] UpdateCouponRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCouponCommand(
            id,
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.UsageLimit,
            request.ExpiresAt);

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Activate or deactivate a coupon.</summary>
    [HttpPatch("{id:int}/toggle")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle(int id, CancellationToken ct)
    {
        var isActive = await _sender.Send(new ToggleCouponCommand(id), ct);
        return Ok(new
        {
            isActive,
            message = isActive
                ? "Coupon activated successfully."
                : "Coupon deactivated successfully."
        });
    }

    /// <summary>Soft delete a coupon.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _sender.Send(new DeleteCouponCommand(id), ct);
        return NoContent();
    }
}

// ─── Request Models ────────────────────────────────────────────────
public record CreateCouponRequest(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    int? UsageLimit,
    DateTime? ExpiresAt);

public record UpdateCouponRequest(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    int? UsageLimit,
    DateTime? ExpiresAt);