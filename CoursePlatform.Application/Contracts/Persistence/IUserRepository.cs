using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Contracts.Persistence;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
}