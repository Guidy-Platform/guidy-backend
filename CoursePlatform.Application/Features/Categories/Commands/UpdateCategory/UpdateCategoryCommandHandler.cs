using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UpdateCategoryCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CategoryDto> Handle(
        UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _uow.Repository<Category>()
                                 .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Category", request.Id);

        // لو الاسم اتغير فعلاً
        if (!string.Equals(category.Name, request.Name,
                StringComparison.OrdinalIgnoreCase))
        {
            // تحقق من تكرار الاسم عند category تانية
            var nameSpec = new CategoryByNameSpec(request.Name);
            var nameExists = await _uow.Repository<Category>()
                                       .AnyAsync(nameSpec, ct);
            if (nameExists)
                throw new ConflictException(
                    $"A category named '{request.Name}' already exists.");

            category.Slug = await SlugHelper.GenerateUniqueSlugAsync(
                request.Name,
                async s =>
                {
                    var spec = new CategoryBySlugExcludingIdSpec(s, request.Id);
                    return await _uow.Repository<Category>().AnyAsync(spec, ct);
                });
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;

        _uow.Repository<Category>().Update(category);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);
        await _cache.RemoveAsync($"categories:{request.Id}", ct);

        var resultSpec = new CategoryWithSubCategoriesSpec(category.Id);
        var result = await _uow.Repository<Category>()
                                   .GetEntityWithSpecAsync(resultSpec, ct);

        return _mapper.Map<CategoryDto>(result!);
    }
}