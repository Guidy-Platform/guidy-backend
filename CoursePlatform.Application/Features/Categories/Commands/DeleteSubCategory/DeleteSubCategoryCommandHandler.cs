using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.DeleteSubCategory;

public class DeleteSubCategoryCommandHandler
    : IRequestHandler<DeleteSubCategoryCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public DeleteSubCategoryCommandHandler(
        IUnitOfWork uow,
        ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        DeleteSubCategoryCommand request, CancellationToken ct)
    {
        var subCategory = await _uow.Repository<SubCategory>()
                                    .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("SubCategory", request.Id);

        if (subCategory.CategoryId != request.CategoryId)
            throw new ForbiddenException(
                "This subcategory does not belong to the specified category.");

        subCategory.Slug = $"{subCategory.Slug}-deleted-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        _uow.Repository<SubCategory>().Delete(subCategory);

        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);
        await _cache.RemoveAsync($"categories:{request.CategoryId}", ct);

        return Unit.Value;
    }
}