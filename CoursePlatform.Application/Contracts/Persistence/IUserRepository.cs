using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Contracts.Persistence;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<AppUser>> GetAllAsync(
        string? search = null,
        bool? isBanned = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetRolesAsync(
        AppUser user, CancellationToken ct = default);
    Task UpdateAsync(AppUser user, CancellationToken ct = default);



}