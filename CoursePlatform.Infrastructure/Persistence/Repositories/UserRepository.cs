using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(
        AppDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<AppUser?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<AppUser?> GetByEmailAsync(
        string email, CancellationToken ct = default)
        => await _context.Users
            .FirstOrDefaultAsync(
                u => u.Email!.ToLower() == email.ToLower(), ct);

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(
        string? search = null,
        bool? isBanned = null,
        CancellationToken ct = default)
        => await _context.Users
            .Where(u =>
                !u.IsDeleted &&
                (string.IsNullOrEmpty(search) ||
                    u.FullName.ToLower().Contains(search.ToLower()) ||
                    u.Email!.ToLower().Contains(search.ToLower())) &&
                (!isBanned.HasValue || u.IsBanned == isBanned.Value))
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        AppUser user, CancellationToken ct = default)
        => (await _userManager.GetRolesAsync(user)).ToList();

    public async Task UpdateAsync(
        AppUser user, CancellationToken ct = default)
        => await _userManager.UpdateAsync(user);
}