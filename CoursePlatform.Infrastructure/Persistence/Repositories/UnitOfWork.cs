// Infrastructure/Persistence/Repositories/UnitOfWork.cs
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Common;
using System;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<string, object> _repos = [];

    public UnitOfWork(AppDbContext context)
        => _context = context;

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var key = typeof(T).Name;

        if (!_repos.TryGetValue(key, out var repo))
        {
            repo = new GenericRepository<T>(_context);
            _repos[key] = repo;
        }

        return (IGenericRepository<T>)repo;
    }

    public async Task<int> CompleteAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async ValueTask DisposeAsync()
        => await _context.DisposeAsync();
}