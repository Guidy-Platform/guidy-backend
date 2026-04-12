using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Queries.GetCourseCurriculum;

public class GetCourseCurriculumQueryHandler
    : IRequestHandler<GetCourseCurriculumQuery, CourseCurriculumDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetCourseCurriculumQueryHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CourseCurriculumDto> Handle(
        GetCourseCurriculumQuery request, CancellationToken ct)
    {
        var spec = new CourseCurriculumSpec(request.CourseId);
        var course = await _uow.Repository<Course>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        // تحديد مين يشوف إيه
        var isOwner = course.InstructorId == _currentUser.UserId;
        var isAdmin = _currentUser.Roles.Contains("Admin");
        var showAll = isOwner || isAdmin || request.IncludeAllLessons;

        // Public يشوف Published بس
        if (!isOwner && !isAdmin &&
            course.Status != CourseStatus.Published)
            throw new NotFoundException("Course", request.CourseId);

        var sections = course.Sections
            .OrderBy(s => s.Order)
            .ToList();

        var sectionDtos = sections.Select(section =>
        {
            var lessons = section.Lessons
                .Where(l => showAll || l.IsFreePreview)  
                .OrderBy(l => l.Order)
                .ToList();

            return new SectionDto
            {
                Id = section.Id,
                Title = section.Title,
                Order = section.Order,
                LessonCount = section.Lessons.Count, 
                TotalSeconds = lessons.Sum(l => l.DurationInSeconds),
                Lessons = _mapper.Map<IList<LessonDto>>(lessons)
            };
        }).ToList();

        return new CourseCurriculumDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            TotalSections = sectionDtos.Count,
            TotalLessons = sectionDtos.Sum(s => s.LessonCount),
            TotalSeconds = sectionDtos.Sum(s => s.TotalSeconds),
            FreePreviewCount = sectionDtos
                .SelectMany(s => s.Lessons)
                .Count(l => l.IsFreePreview),
            Sections = sectionDtos
        };
    }
}