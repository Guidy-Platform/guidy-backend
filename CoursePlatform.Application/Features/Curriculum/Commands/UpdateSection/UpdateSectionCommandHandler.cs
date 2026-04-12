using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.UpdateSection;

public class UpdateSectionCommandHandler
    : IRequestHandler<UpdateSectionCommand, SectionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UpdateSectionCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<SectionDto> Handle(
        UpdateSectionCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        var section = await _uow.Repository<Section>()
                                .GetByIdAsync(request.SectionId, ct)
            ?? throw new NotFoundException("Section", request.SectionId);

        if (section.CourseId != request.CourseId)
            throw new ForbiddenException(
                "Section does not belong to this course.");

        section.Title = request.Title;

        _uow.Repository<Section>().Update(section);
        await _uow.CompleteAsync(ct);

        return _mapper.Map<SectionDto>(section);
    }
}