using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Payouts.Commands.ApprovePayout;
using CoursePlatform.Application.Features.Payouts.Commands.RejectPayout;
using CoursePlatform.Application.Features.Payouts.Commands.RequestPayout;
using CoursePlatform.Application.Features.Payouts.DTOs;
using CoursePlatform.Application.Features.Payouts.Queries.GetAllPayouts;
using CoursePlatform.Application.Features.Payouts.Queries.GetMyPayouts;
using CoursePlatform.Application.Features.Payouts.Queries.GetMyWallet;
using CoursePlatform.Application.Features.Payouts.Helpers;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/payouts")]
[Authorize]
public class PayoutsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IStripeConnectService _stripe;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public PayoutsController(
        ISender sender,
        IStripeConnectService stripe,
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _sender = sender;
        _stripe = stripe;
        _uow = uow;
        _currentUser = currentUser;
    }

    // ─── Instructor ───────────────────────────────────────────────

    /// <summary>Get my wallet balance and earnings breakdown.</summary>
    [HttpGet("wallet")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WalletDto>> GetWallet(CancellationToken ct)
        => Ok(await _sender.Send(new GetMyWalletQuery(), ct));

    /// <summary>Get my payout history.</summary>
    [HttpGet("my")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(IReadOnlyList<PayoutDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PayoutDto>>> GetMyPayouts(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetMyPayoutsQuery(), ct));

    /// <summary>
    /// Connect Stripe account.
    /// Returns onboarding URL for Stripe Express.
    /// </summary>
    [HttpPost("connect-stripe")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConnectStripe(CancellationToken ct)
    {
        var instructorId = _currentUser.UserId!.Value;

        var wallet = await WalletHelper.GetOrCreateWalletAsync(
            instructorId, _uow, ct);

        if (wallet.IsStripeConnected)
            return Ok(new
            {
                message = "Stripe already connected.",
                stripeAccountId = wallet.StripeAccountId,
                isAlreadyConnected = true
            });

        // get the instructor's email
        var userRepo = HttpContext.RequestServices
            .GetRequiredService<IUserRepository>();
        var instructor = await userRepo.GetByIdAsync(instructorId, ct);

        var result = await _stripe.CreateConnectAccountAsync(
            instructor?.Email ?? string.Empty, ct);

        // save the Stripe Account ID
        wallet.StripeAccountId = result.AccountId;
        wallet.IsStripeConnected = true;
        _uow.Repository<InstructorWallet>().Update(wallet);
        await _uow.CompleteAsync(ct);

        return Ok(new
        {
            message = "Stripe account created. Complete onboarding.",
            onboardingUrl = result.OnboardingUrl,
            stripeAccountId = result.AccountId
        });
    }

    /// <summary>
    /// Request a payout. Minimum $50.
    /// Must have Stripe account connected.
    /// </summary>
    [HttpPost("request")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(PayoutDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayoutDto>> RequestPayout(
        [FromBody] RequestPayoutRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RequestPayoutCommand(request.Amount), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    // ─── Admin ────────────────────────────────────────────────────

    /// <summary>Get all payout requests. Filter by status.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<PayoutDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PayoutDto>>> GetAll(
        [FromQuery] PayoutStatus? status,
        CancellationToken ct)
        => Ok(await _sender.Send(new GetAllPayoutsQuery(status), ct));

    /// <summary>
    /// Approve payout and trigger Stripe transfer.
    /// </summary>
    [HttpPut("{payoutId:int}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayoutDto>> Approve(
        int payoutId,
        [FromBody] ApprovePayoutRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ApprovePayoutCommand(payoutId, request.Notes), ct);
        return Ok(result);
    }

    /// <summary>Reject a payout request with reason.</summary>
    [HttpPut("{payoutId:int}/reject")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PayoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayoutDto>> Reject(
        int payoutId,
        [FromBody] RejectPayoutRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new RejectPayoutCommand(payoutId, request.Reason), ct);
        return Ok(result);
    }
}

// ─── Request Models ────────────────────────────────────────────────
public record RequestPayoutRequest(decimal Amount);
public record ApprovePayoutRequest(string? Notes = null);
public record RejectPayoutRequest(string Reason);