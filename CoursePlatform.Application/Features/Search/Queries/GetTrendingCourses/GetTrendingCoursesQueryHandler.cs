using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetTrendingCourses;

public class GetTrendingCoursesQueryHandler
    : IRequestHandler<GetTrendingCoursesQuery, IReadOnlyList<CourseSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetTrendingCoursesQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CourseSummaryDto>> Handle(
        GetTrendingCoursesQuery request, CancellationToken ct)
    {
        var cacheKey = $"courses:trending:{request.Take}";
        var cached = await _cache.GetAsync<IReadOnlyList<CourseSummaryDto>>(
            cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new TrendingCoursesSpec(request.Take);
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        var result = _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);

        // Cache for 30 minutes
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30), ct);

        return result;
    }
}