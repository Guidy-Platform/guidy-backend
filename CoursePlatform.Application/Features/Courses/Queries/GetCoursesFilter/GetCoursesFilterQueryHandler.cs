using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Courses.Queries.GetMyCourses;
using CoursePlatform.Application.Features.Courses.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetCoursesFilter;

public class GetCoursesFilterQueryHandler
    : IRequestHandler<GetCoursesFilterQuery,
                      IReadOnlyList<CourseFilterItemDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCoursesFilterQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<IReadOnlyList<CourseFilterItemDto>> Handle(
        GetCoursesFilterQuery request, CancellationToken ct)
    {
        var spec = new AllCoursesFilterSpec(request.Search);
        var courses = await _uow.Repository<Course>()
                                .GetAllWithSpecAsync(spec, ct);

        return courses
            .Select(c => new CourseFilterItemDto
            {
                Id = c.Id,
                Title = c.Title
            })
            .ToList();
    }
}