using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Courses.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetPendingCourses;

public class GetPendingCoursesQueryHandler
    : IRequestHandler<GetPendingCoursesQuery, IReadOnlyList<CourseSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetPendingCoursesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CourseSummaryDto>> Handle(
        GetPendingCoursesQuery request, CancellationToken ct)
    {
        var spec = new PendingCoursesSpec();
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        return _mapper.Map<IReadOnlyList<CourseSummaryDto>>(courses);
    }
}