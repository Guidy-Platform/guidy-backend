using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetPopularCourses;

public class GetPopularCoursesQueryHandler
    : IRequestHandler<GetPopularCoursesQuery, IReadOnlyList<CourseSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetPopularCoursesQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CourseSummaryDto>> Handle(
        GetPopularCoursesQuery request, CancellationToken ct)
    {
        var cacheKey = $"courses:popular:{request.Take}";
        var cached = await _cache.GetAsync<IReadOnlyList<CourseSummaryDto>>(
            cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new PopularCoursesSpec(request.Take);
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        var result = _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1), ct);

        return result;
    }
}