// Infrastructure/Services/AuthService.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Auth.DTOs;
using CoursePlatform.Application.Features.Auth.Events;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMessagePublisher _publisher;
    private readonly IGoogleAuthService _googleAuth;
    private readonly AppDbContext _context;

    private readonly IOtpService _otpService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IMessagePublisher publisher,
        IGoogleAuthService googleAuth,
        IOtpService otpService,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _publisher = publisher;
        _googleAuth = googleAuth;
        _otpService = otpService;
        _context = context;
    }

    public async Task RegisterAsync(
      string firstName, string lastName,
      string email, string password,
      string role, CancellationToken ct = default)
    {
        // 1. التحقق من عدم تكرار الإيميل
        var exists = await _userManager.FindByEmailAsync(email);
        if (exists is not null)
            throw new ConflictException($"Email '{email}' is already registered.");

        // 2. إنشاء الـ user
        var user = new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            throw new BadRequestException(
                string.Join(", ", createResult.Errors.Select(e => e.Description)));

        // 3. تعيين الـ Role
        await _userManager.AddToRoleAsync(user, role);

        // 4. توليد OTP وحفظه
        var otpCode = await _otpService.GenerateAndSaveOtpAsync(user.Id, ct);

        // 5. إرسال OTP عبر RabbitMQ
        await _publisher.PublishAsync(new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            OtpCode = otpCode
        }, "user.registered", ct);
    }

    public async Task<AuthResponseDto> LoginAsync(
        string email, string password,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (user.IsDeleted)
            throw new UnauthorizedException(
                "This account has been deactivated.");

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new UnauthorizedException(
                "Please confirm your email before logging in.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedException(
                "Account is temporarily locked. Try again later.");

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = await _tokenService.CreateAccessTokenAsync(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return MapToDto(user, accessToken, refreshToken.Token, roles);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        string accessToken, string refreshToken,
        CancellationToken ct = default)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(accessToken)
            ?? throw new UnauthorizedException("Invalid access token.");

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedException("User not found.");

        var stored = await _tokenService
            .GetActiveRefreshTokenAsync(user.Id, refreshToken, ct)
            ?? throw new UnauthorizedException(
                "Invalid or expired refresh token.");

        await _tokenService.RevokeRefreshTokenAsync(stored, ct);

        var newAccess = await _tokenService.CreateAccessTokenAsync(user);
        var newRefresh = await _tokenService.CreateRefreshTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return MapToDto(user, newAccess, newRefresh.Token, roles);
    }

    public async Task VerifyEmailAsync(
      string email, string code,
      CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("User", email);

        if (user.EmailConfirmed)
            throw new BadRequestException("Email is already verified.");

        var isValid = await _otpService.ValidateOtpAsync(user.Id, code, ct);
        if (!isValid)
            throw new BadRequestException("Invalid or expired OTP code.");

        // تأكيد الإيميل يدوياً
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
    }

    // في Infrastructure/Services/AuthService.cs

    public async Task ForgotPasswordAsync(
        string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // لا نكشف معلومات — security best practice
        if (user is null || !user.EmailConfirmed || user.IsDeleted)
            return;

        // OTP مش token
        var otpCode = await _otpService.GenerateAndSaveOtpAsync(user.Id, ct);

        await _publisher.PublishAsync(new PasswordResetRequestedEvent
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            OtpCode = otpCode       // ← OTP
        }, "password.reset.requested", ct);
    }

    public async Task ResetPasswordAsync(
        string email, string otpCode,
        string newPassword, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("User", email);

        // 1. التحقق من الـ OTP
        var isValid = await _otpService.ValidateOtpAsync(user.Id, otpCode, ct);
        if (!isValid)
            throw new BadRequestException("Invalid or expired OTP code.");

        // 2. reset الـ password بدون token
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(
                             user, resetToken, newPassword);

        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    public async Task ChangePasswordAsync(
        Guid userId, string currentPassword,
        string newPassword, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var result = await _userManager
            .ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(
        string idToken, CancellationToken ct = default)
    {
        var payload = await _googleAuth.VerifyTokenAsync(idToken)
            ?? throw new UnauthorizedException("Invalid Google token.");

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new AppUser
            {
                Email = payload.Email,
                UserName = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Student");
        }

        if (user.IsDeleted)
            throw new UnauthorizedException(
                "This account has been deactivated.");

        var accessToken = await _tokenService.CreateAccessTokenAsync(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return MapToDto(user, accessToken, refreshToken.Token, roles);
    }

    public async Task RevokeTokenAsync(
        Guid userId, string refreshToken,
        CancellationToken ct = default)
    {
        var token = await _tokenService
            .GetActiveRefreshTokenAsync(userId, refreshToken, ct)
            ?? throw new BadRequestException(
                "Token not found or already revoked.");

        await _tokenService.RevokeRefreshTokenAsync(token, ct);
    }

    public async Task ResendOtpAsync(
    string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("User", email);

        if (user.EmailConfirmed)
            throw new BadRequestException("Email is already verified.");

        var otpCode = await _otpService.GenerateAndSaveOtpAsync(user.Id, ct);

        await _publisher.PublishAsync(new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            OtpCode = otpCode
        }, "user.registered", ct);
    }

    // ─── Private Helper ───────────────────────────────────────────────────
    private static AuthResponseDto MapToDto(
        AppUser user, string accessToken,
        string refreshToken, IList<string> roles)
        => new()
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            Roles = roles
        };
}