using CoursePlatform.Application.Features.Enrollments.Queries.GetMyEnrollments;
using CoursePlatform.Application.Features.Orders.Commands.ApplyCoupon;
using CoursePlatform.Application.Features.Orders.Commands.CreateOrder;
using CoursePlatform.Application.Features.Orders.DTOs;
using CoursePlatform.Application.Features.Orders.Queries.GetMyOrders;
using CoursePlatform.Application.Features.Enrollments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
        => _sender = sender;

    /// <summary>
    /// Create order for one or more courses.
    /// Returns client_secret for Stripe payment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateOrderResultDto>> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Apply a coupon to a pending order.</summary>
    [HttpPost("{orderId:int}/coupon")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> ApplyCoupon(
        int orderId,
        [FromBody] ApplyCouponRequest request,
        CancellationToken ct)
    {
        var command = new ApplyCouponCommand(orderId, request.CouponCode);
        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Get all my orders.</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetMyOrders(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyOrdersQuery(), ct));

    ///// <summary>Get all my enrollments.</summary>
    //[HttpGet("my/enrollments")]
    //[ProducesResponseType(typeof(IReadOnlyList<EnrollmentDto>), StatusCodes.Status200OK)]
    //public async Task<ActionResult<IReadOnlyList<EnrollmentDto>>> GetMyEnrollments(
    //    CancellationToken ct)
    //    => Ok(await _sender.Send(new GetMyEnrollmentsQuery(), ct));
}

public record ApplyCouponRequest(string CouponCode);