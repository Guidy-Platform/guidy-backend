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
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email!.ToLower().Contains(search));
        }

        if (isBanned.HasValue)
        {
            query = query.Where(u => u.IsBanned == isBanned.Value);
        }

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<string>> GetRolesAsync(
        AppUser user, CancellationToken ct = default)
        => (await _userManager.GetRolesAsync(user)).ToList();

    public async Task UpdateAsync(
        AppUser user, CancellationToken ct = default)
        => await _userManager.UpdateAsync(user);
}