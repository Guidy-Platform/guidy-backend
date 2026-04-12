using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.ReorderCategories;

public class ReorderCategoriesCommandHandler
    : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public ReorderCategoriesCommandHandler(
        IUnitOfWork uow,
        ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        ReorderCategoriesCommand request, CancellationToken ct)
    {
        foreach (var item in request.Items)
        {
            var category = await _uow.Repository<Category>()
                                     .GetByIdAsync(item.Id, ct)
                ?? throw new NotFoundException("Category", item.Id);

            category.Order = item.Order;
            _uow.Repository<Category>().Update(category);
        }

        await _uow.CompleteAsync(ct);
        await _cache.RemoveAsync("categories:all", ct);

        return Unit.Value;
    }
}