using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.DTOs;
using CoursePlatform.Application.Features.Search.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.SearchCourses;

public class SearchCoursesQueryHandler
    : IRequestHandler<SearchCoursesQuery, SearchResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public SearchCoursesQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<SearchResultDto> Handle(
        SearchCoursesQuery request, CancellationToken ct)
    {
        var p = request.Params;

        var cacheKey = $"search:{p.Search}:{p.CategoryId}:{p.SubCategoryId}:" +
                       $"{p.Level}:{p.Language}:{p.MinPrice}:{p.MaxPrice}:" +
                       $"{p.SortBy}:{p.PageIndex}:{p.PageSize}";

        var cached = await _cache.GetAsync<SearchResultDto>(cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new SearchCoursesSpec(p);
        var countSpec = new SearchCoursesCountSpec(p);

        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);
        var total = await _uow.Repository<Course>()
                                .CountAsync(countSpec, ct);

        var dtos = _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);
        var pages = (int)Math.Ceiling((double)total / p.PageSize);

        var result = new SearchResultDto
        {
            Total = total,
            PageIndex = p.PageIndex,
            PageSize = p.PageSize,
            TotalPages = pages,
            Query = p.Search,
            Courses = dtos,
            AppliedFilters = new SearchFiltersDto
            {
                Search = p.Search,
                CategoryId = p.CategoryId,
                SubCategoryId = p.SubCategoryId,
                Level = p.Level?.ToString(),
                Language = p.Language,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                SortBy = p.SortBy
            }
        };

        // cache key for 5 minutes only for search results
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), ct);

        return result;
    }
}