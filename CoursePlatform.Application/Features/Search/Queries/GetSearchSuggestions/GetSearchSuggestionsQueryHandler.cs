using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Search.DTOs;
using CoursePlatform.Application.Features.Search.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetSearchSuggestions;

public class GetSearchSuggestionsQueryHandler
    : IRequestHandler<GetSearchSuggestionsQuery,
                      IReadOnlyList<SearchSuggestionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public GetSearchSuggestionsQueryHandler(
        IUnitOfWork uow,
        ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<IReadOnlyList<SearchSuggestionDto>> Handle(
        GetSearchSuggestionsQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query) ||
            request.Query.Length < 2)
            return [];

        var cacheKey = $"suggestions:{request.Query.ToLower()}";
        var cached = await _cache.GetAsync<IReadOnlyList<SearchSuggestionDto>>(
            cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new SuggestionsSpec(request.Query, take: 5);
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        var result = courses.Select(c => new SearchSuggestionDto
        {
            CourseId = c.Id,
            Title = c.Title,
            Category = c.SubCategory?.Category?.Name ?? string.Empty,
            ThumbnailUrl = c.ThumbnailUrl
        }).ToList();

        await _cache.SetAsync(
            cacheKey, result, TimeSpan.FromMinutes(10), ct);

        return result;
    }
}