using CoursePlatform.Domain.Common;

namespace CoursePlatform.Application.Contracts.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> CompleteAsync(CancellationToken ct = default);
}