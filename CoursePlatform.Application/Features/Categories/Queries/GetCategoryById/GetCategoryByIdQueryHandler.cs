using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetCategoryByIdQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CategoryDto> Handle(
        GetCategoryByIdQuery request, CancellationToken ct)
    {
        var cacheKey = $"categories:{request.Id}";

        var cached = await _cache.GetAsync<CategoryDto>(cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new CategoryWithSubCategoriesSpec(request.Id);
        var category = await _uow.Repository<Category>()
                                 .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Category", request.Id);

        var result = _mapper.Map<CategoryDto>(category);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(6), ct);

        return result;
    }
}