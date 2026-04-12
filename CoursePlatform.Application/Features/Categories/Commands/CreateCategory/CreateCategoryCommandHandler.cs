using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateCategoryCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }
    // Application/Features/Categories/Commands/CreateCategory/CreateCategoryCommandHandler.cs
    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request, CancellationToken ct)
    {
        // التحقق من الاسم
        var nameExists = await _uow.Repository<Category>()
                                   .AnyAsync(new CategoryByNameSpec(request.Name), ct);
        if (nameExists)
            throw new ConflictException(
                $"A category named '{request.Name}' already exists.");

        // slug فريد
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            request.Name,
            async s => await _uow.Repository<Category>()
                                 .AnyAsync(new CategoryBySlugSpec(s), ct));

        // Auto-assign order — آخر category + 1
        var maxOrderSpec = new AllCategoriesWithSubCategoriesSpec();
        var allCategories = await _uow.Repository<Category>()
                                      .GetAllWithSpecAsync(maxOrderSpec, ct);
        var nextOrder = allCategories.Any()
            ? allCategories.Max(c => c.Order) + 1
            : 1;

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            IconUrl = request.IconUrl,
            Order = nextOrder   // auto-assign order
        };

        await _uow.Repository<Category>().AddAsync(category, ct);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);

        var spec = new CategoryWithSubCategoriesSpec(category.Id);
        var result = await _uow.Repository<Category>()
                               .GetEntityWithSpecAsync(spec, ct);

        return _mapper.Map<CategoryDto>(result!);
    }
}