using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public DeleteCategoryCommandHandler(
        IUnitOfWork uow,
        ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _uow.Repository<Category>()
                                 .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Category", request.Id);

        category.Slug = $"{category.Slug}-deleted-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        _uow.Repository<Category>().Delete(category);
      

        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);
        await _cache.RemoveAsync($"categories:{request.Id}", ct);

        return Unit.Value;
    }
}