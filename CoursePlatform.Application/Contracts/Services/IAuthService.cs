// Application/Contracts/Services/IAuthService.cs
using CoursePlatform.Application.Features.Auth.Commands.ResendOtp;
using CoursePlatform.Application.Features.Auth.DTOs;

public interface IAuthService
{
    Task RegisterAsync(
        string firstName, string lastName,
        string email, string password,
        string role, CancellationToken ct = default);

    Task<AuthResponseDto> LoginAsync(
        string email, string password,
        CancellationToken ct = default);

    Task<AuthResponseDto> RefreshTokenAsync(
        string accessToken, string refreshToken,
        CancellationToken ct = default);

    Task VerifyEmailAsync(
        string email, string code,          // ← code مش token
        CancellationToken ct = default);

    Task ResendOtpAsync(
        string email,
        OtpPurpose purpose,
        CancellationToken ct = default);

    Task ForgotPasswordAsync(
        string email,
        CancellationToken ct = default);

    Task ResetPasswordAsync(
        string email, string otpCode,
        string newPassword, CancellationToken ct = default);

    Task ChangePasswordAsync(
        Guid userId, string currentPassword,
        string newPassword, CancellationToken ct = default);

    Task<AuthResponseDto> GoogleLoginAsync(
        string idToken,
        CancellationToken ct = default);

    Task RevokeTokenAsync(
        Guid userId, string refreshToken,
        CancellationToken ct = default);
}