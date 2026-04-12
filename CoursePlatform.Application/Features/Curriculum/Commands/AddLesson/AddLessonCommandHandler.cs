using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddLesson;

public class AddLessonCommandHandler
    : IRequestHandler<AddLessonCommand, LessonDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public AddLessonCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<LessonDto> Handle(
        AddLessonCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        var section = await _uow.Repository<Section>()
                                .GetByIdAsync(request.SectionId, ct)
            ?? throw new NotFoundException("Section", request.SectionId);

        if (section.CourseId != request.CourseId)
            throw new ForbiddenException(
                "Section does not belong to this course.");

        // Auto-assign order داخل الـ Section
        var existingLessons = await _uow.Repository<Lesson>()
            .GetAllWithSpecAsync(
                new LessonsBySectionSpec(request.SectionId), ct);

        var nextOrder = existingLessons.Any()
            ? existingLessons.Max(l => l.Order) + 1
            : 1;

        var lesson = new Lesson
        {
            SectionId = request.SectionId,
            Title = request.Title,
            Description = request.Description,
            VideoUrl = request.VideoUrl,
            DurationInSeconds = request.DurationInSeconds,
            Type = request.Type,
            IsFreePreview = request.IsFreePreview,
            Order = nextOrder
        };

        await _uow.Repository<Lesson>().AddAsync(lesson, ct);
        await _uow.CompleteAsync(ct);

        return _mapper.Map<LessonDto>(lesson);
    }
}