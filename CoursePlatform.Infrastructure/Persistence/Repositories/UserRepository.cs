using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
        => _context = context;

    public async Task<AppUser?> GetByIdAsync(
        Guid id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(
            u => u.Id == id, ct);
}