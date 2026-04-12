using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.UpdateLesson;

public class UpdateLessonCommandHandler
    : IRequestHandler<UpdateLessonCommand, LessonDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UpdateLessonCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<LessonDto> Handle(
        UpdateLessonCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        var lesson = await _uow.Repository<Lesson>()
                               .GetByIdAsync(request.LessonId, ct)
            ?? throw new NotFoundException("Lesson", request.LessonId);

        if (lesson.SectionId != request.SectionId)
            throw new ForbiddenException(
                "Lesson does not belong to this section.");

        lesson.Title = request.Title;
        lesson.Description = request.Description;
        lesson.VideoUrl = request.VideoUrl;
        lesson.DurationInSeconds = request.DurationInSeconds;
        lesson.Type = request.Type;
        lesson.IsFreePreview = request.IsFreePreview;

        _uow.Repository<Lesson>().Update(lesson);
        await _uow.CompleteAsync(ct);

        return _mapper.Map<LessonDto>(lesson);
    }
}