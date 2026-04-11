using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
        => _context = context;

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Set<T>().FindAsync([id], ct);

    public async Task<T?> GetEntityWithSpecAsync(
        ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _context.Set<T>().AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(
        ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpec(spec).AnyAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _context.Set<T>().AddAsync(entity, ct);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
    // AuditInterceptor  authomatic use soft delete

    public async Task AddRangeAsync(
        IEnumerable<T> entities, CancellationToken ct = default)
        => await _context.Set<T>().AddRangeAsync(entities, ct);

    private IQueryable<T> ApplySpec(ISpecification<T> spec)
        => SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
}