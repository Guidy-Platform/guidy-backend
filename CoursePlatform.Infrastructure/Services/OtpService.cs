// Infrastructure/Services/OtpService.cs
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace CoursePlatform.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly AppDbContext _context;

    public OtpService(AppDbContext context)
        => _context = context;

    public async Task<string> GenerateAndSaveOtpAsync(
        Guid userId, CancellationToken ct = default)
    {
        // إلغاء كل الـ OTPs القديمة للـ user
        var old = await _context.EmailOtps
            .Where(o => o.UserId == userId && !o.IsUsed)
            .ToListAsync(ct);

        _context.EmailOtps.RemoveRange(old);

        // توليد 6 أرقام
        var code = Random.Shared.Next(100_000, 999_999).ToString();

        var otp = new EmailOtp
        {
            Code = code,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),  // صالح 10 دقائق
        };

        await _context.EmailOtps.AddAsync(otp, ct);
        await _context.SaveChangesAsync(ct);

        return code;
    }

    public async Task<bool> ValidateOtpAsync(
        Guid userId, string code, CancellationToken ct = default)
    {
        var otp = await _context.EmailOtps
            .Where(o =>
                o.UserId == userId &&
                o.Code == code &&
                !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (otp is null || !otp.IsValid)
            return false;

        // mark as used
        otp.IsUsed = true;
        await _context.SaveChangesAsync(ct);

        return true;
    }
}