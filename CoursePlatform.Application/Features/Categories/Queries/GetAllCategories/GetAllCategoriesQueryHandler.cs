using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    private const string CacheKey = "categories:all";

    public GetAllCategoriesQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(
        GetAllCategoriesQuery request, CancellationToken ct)
    {
        // will be changed rarely — cache it for a long time
        var cached = await _cache.GetAsync<IReadOnlyList<CategoryDto>>(CacheKey, ct);
        if (cached is not null) return cached;

        var spec = new AllCategoriesWithSubCategoriesSpec();
        var categories = await _uow.Repository<Category>()
                                   .GetAllWithSpecAsync(spec, ct);

        var result = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

        await _cache.SetAsync(CacheKey, result, TimeSpan.FromHours(6), ct);

        return result;
    }
}