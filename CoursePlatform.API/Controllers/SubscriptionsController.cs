using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Subscriptions.Commands.CancelSubscription;
using CoursePlatform.Application.Features.Subscriptions.Commands.HandleSubscriptionWebhook;
using CoursePlatform.Application.Features.Subscriptions.Commands.Subscribe;
using CoursePlatform.Application.Features.Subscriptions.DTOs;
using CoursePlatform.Application.Features.Subscriptions.Queries.GetMySubscription;
using CoursePlatform.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IStripeSubscriptionService _stripe;
    private readonly IUnitOfWork _uow;

    public SubscriptionsController(
        ISender sender,
        IStripeSubscriptionService stripe,
        IUnitOfWork uow)
    {
        _sender = sender;
        _stripe = stripe;
        _uow = uow;
    }

    /// <summary>Get all available subscription plans.</summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<SubscriptionPlanDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SubscriptionPlanDto>>> GetPlans(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetSubscriptionPlansQuery(), ct));

    /// <summary>Get my current subscription.</summary>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(UserSubscriptionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSubscriptionDto?>> GetMine(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMySubscriptionQuery(), ct));

    /// <summary>
    /// Subscribe to a plan.
    /// Returns client_secret for Stripe payment confirmation.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(UserSubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserSubscriptionDto>> Subscribe(
        [FromBody] SubscribeRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new SubscribeCommand(request.PlanId), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Cancel subscription (at period end, not immediately).
    /// </summary>
    [HttpDelete("my")]
    [Authorize]
    [ProducesResponseType(typeof(UserSubscriptionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSubscriptionDto>> Cancel(
        CancellationToken ct)
        => Ok(await _sender.Send(new CancelSubscriptionCommand(), ct));

    /// <summary>
    /// Stripe Subscription Webhook.
    /// Handles renewals and cancellations.
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        string payload;
        Request.Body.Position = 0;
        using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            payload = await reader.ReadToEndAsync(ct);

        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

        var isValid = _stripe.ValidateWebhookSignature(
            payload, signature,
            out var eventType,
            out var subscriptionId,
            out var periodEnd);

        if (!isValid)
            return BadRequest(new { message = "Invalid webhook signature." });

        switch (eventType)
        {
            // Subscription renewed successfully
            case "invoice.paid":
            case "customer.subscription.updated":
                if (!string.IsNullOrEmpty(subscriptionId) && periodEnd.HasValue)
                    await _sender.Send(
                        new HandleSubscriptionRenewedCommand(
                            subscriptionId, periodEnd.Value), ct);
                break;

            // Subscription ended
            case "customer.subscription.deleted":
                if (!string.IsNullOrEmpty(subscriptionId))
                    await _sender.Send(
                        new HandleSubscriptionCancelledCommand(
                            subscriptionId), ct);
                break;
        }

        return Ok();
    }



#if DEBUG
    /// <summary>DEV ONLY — Simulate subscription renewal.</summary>
    [HttpPost("test/simulate-renewal")]
    [Authorize]
    public async Task<IActionResult> SimulateRenewal(
        CancellationToken ct)
    {
        var sub = await _sender.Send(new GetMySubscriptionQuery(), ct);
        if (sub is null)
            return BadRequest(new { message = "No subscription found." });

        // get the active subscription entity for the user
        var userId = HttpContext.User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var subSpec = new ActiveSubscriptionByUserSpec(Guid.Parse(userId!));
        var subEntity = await _uow.Repository<UserSubscription>()
                                  .GetEntityWithSpecAsync(subSpec, ct);

        if (subEntity is null)
            return BadRequest(new { message = "No active subscription found." });

        await _sender.Send(new HandleSubscriptionRenewedCommand(
            subEntity.StripeSubscriptionId!,
            DateTime.UtcNow.AddMonths(1)), ct);

        return Ok(new { message = "Renewal simulated successfully." });
    }

    /// <summary>DEV ONLY — Simulate subscription expired.</summary>
    [HttpPost("test/simulate-expired")]
    [Authorize]
    public async Task<IActionResult> SimulateExpired(CancellationToken ct)
    {
        var userId = HttpContext.User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var subSpec = new ActiveSubscriptionByUserSpec(Guid.Parse(userId!));
        var subEntity = await _uow.Repository<UserSubscription>()
                                  .GetEntityWithSpecAsync(subSpec, ct);

        if (subEntity is null)
            return BadRequest(new { message = "No active subscription found." });

        await _sender.Send(new HandleSubscriptionCancelledCommand(
            subEntity.StripeSubscriptionId!), ct);

        return Ok(new { message = "Subscription expired simulated." });
    }
#endif

}

public record SubscribeRequest(int PlanId);