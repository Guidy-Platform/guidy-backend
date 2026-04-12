using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Courses.DTOs;
using CoursePlatform.Application.Features.Courses.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler
    : IRequestHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUser;

    public GetCourseByIdQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
        _currentUser = currentUser;
    }

    public async Task<CourseDto> Handle(
        GetCourseByIdQuery request, CancellationToken ct)
    {
        var cacheKey = $"courses:detail:{request.Id}";

        var cached = await _cache.GetAsync<CourseDto>(cacheKey, ct);
        if (cached is not null) return cached;

        var spec = new CourseByIdSpec(request.Id);
        var course = await _uow.Repository<Course>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Course", request.Id);

        // Public users يشوفوا Published بس
        var isInstructorOrAdmin =
            _currentUser.Roles.Contains("Instructor") ||
            _currentUser.Roles.Contains("Admin");

        if (!isInstructorOrAdmin && course.Status != CourseStatus.Published)
            throw new NotFoundException("Course", request.Id);

        // Instructor يشوف كورسه بس
        if (_currentUser.Roles.Contains("Instructor") &&
            course.InstructorId != _currentUser.UserId)
            throw new ForbiddenException();

        var dto = _mapper.Map<CourseDto>(course);

        // نكيش بس الـ Published courses
        if (course.Status == CourseStatus.Published)
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);

        return dto;
    }
}