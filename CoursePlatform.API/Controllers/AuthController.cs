using CoursePlatform.API.Extensions;
using CoursePlatform.Application.Features.Auth.Commands.ChangePassword;
using CoursePlatform.Application.Features.Auth.Commands.ForgotPassword;
using CoursePlatform.Application.Features.Auth.Commands.GoogleLogin;
using CoursePlatform.Application.Features.Auth.Commands.Login;
using CoursePlatform.Application.Features.Auth.Commands.RefreshToken;
using CoursePlatform.Application.Features.Auth.Commands.Register;
using CoursePlatform.Application.Features.Auth.Commands.ResendOtp;
using CoursePlatform.Application.Features.Auth.Commands.ResetPassword;
using CoursePlatform.Application.Features.Auth.Commands.RevokeToken;
using CoursePlatform.Application.Features.Auth.Commands.VerifyEmail;
using CoursePlatform.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
        => _sender = sender;

    // API/Controllers/AuthController.cs

    /// <summary>Register — returns success message + sends OTP to email.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "User created successfully. Please verify your email."
        });
    }

    /// <summary>Verify email using the 6-digit OTP sent to inbox.</summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return Ok(new { message = "Email verified successfully. You can now log in." });
    }

    /// <summary>Resend OTP code to email.</summary>
    [HttpPost("resend-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return Ok(new { message = "A new OTP code has been sent to your email." });
    }
    /// <summary>Login and receive JWT + refresh token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    /// <summary>Rotate access token using refresh token.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    

    /// <summary>Send password reset email.</summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        // The same response whether the email exists or not — security
        return Ok(new { message = "If this email is registered, a reset link has been sent." });
    }

    /// <summary>Reset password using the token from email.</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return Ok(new { message = "Password reset successfully." });
    }

    /// <summary>Change password for the authenticated user.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return Ok(new { message = "Password changed successfully." });
    }

    /// <summary>Login or register using Google OAuth.</summary>
    [HttpPost("google")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin(
        [FromBody] GoogleLoginCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    /// <summary>Revoke refresh token (logout).</summary>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RevokeTokenCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return Ok(new { message = "Token revoked successfully." });
    }

    /// <summary>Get current authenticated user info from JWT claims.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me() => Ok(new
    {
        Id = User.GetUserId(),
        Email = User.GetEmail(),
        Roles = User.GetRoles(),
        FirstName = User.FindFirstValue(ClaimTypes.GivenName),
        LastName = User.FindFirstValue(ClaimTypes.Surname),
    });
}