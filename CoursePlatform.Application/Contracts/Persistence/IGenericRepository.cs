// Application/Contracts/Persistence/IGenericRepository.cs
using CoursePlatform.Application.Common.Models;
using CoursePlatform.Domain.Common;

namespace CoursePlatform.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : BaseEntity
{
    // Single
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec, CancellationToken ct = default);

    // List
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec, CancellationToken ct = default);

    // Count
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default); // returns the total number of items that match the specification, ignoring pagination
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken ct = default); // returns true if any items match the specification, false otherwise

    // Write
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
}