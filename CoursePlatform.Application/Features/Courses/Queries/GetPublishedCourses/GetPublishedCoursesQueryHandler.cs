using AutoMapper;
using CoursePlatform.Application.Common.Models;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Courses.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetPublishedCourses;

public class GetPublishedCoursesQueryHandler
    : IRequestHandler<GetPublishedCoursesQuery, Pagination<CourseSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetPublishedCoursesQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Pagination<CourseSummaryDto>> Handle(
        GetPublishedCoursesQuery request, CancellationToken ct)
    {
        var p = request.Params;

        var cacheKey = $"courses:published:{p.PageIndex}:{p.PageSize}:" +
                       $"{p.Search}:{p.SubCategoryId}:{p.CategoryId}:" +
                       $"{p.Level}:{p.Language}:{p.MinPrice}:{p.MaxPrice}:{p.SortBy}";

        var cached = await _cache.GetAsync<Pagination<CourseSummaryDto>>(cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new PublishedCoursesSpec(p);
        var countSpec = new PublishedCoursesCountSpec(p);

        var courses = await _uow.Repository<Course>().GetAllWithSpecAsync(spec, ct);
        var total = await _uow.Repository<Course>().CountAsync(countSpec, ct);

        var dtos = _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);
        var result = new Pagination<CourseSummaryDto>(
            p.PageIndex, p.PageSize, total, dtos);

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10), ct);

        return result;
    }
}