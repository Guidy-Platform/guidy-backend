using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Search.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetRelatedCourses;

public class GetRelatedCoursesQueryHandler
    : IRequestHandler<GetRelatedCoursesQuery, IReadOnlyList<CourseSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetRelatedCoursesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CourseSummaryDto>> Handle(
        GetRelatedCoursesQuery request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        var spec = new RelatedCoursesSpec(
            course.SubCategoryId, request.CourseId, take: 6);
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        return _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);
    }
}