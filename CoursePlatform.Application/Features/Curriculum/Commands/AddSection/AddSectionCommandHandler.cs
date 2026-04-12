using AutoMapper;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Application.Features.Curriculum.Helpers;
using CoursePlatform.Application.Features.Curriculum.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Curriculum.Commands.AddSection;

public class AddSectionCommandHandler
    : IRequestHandler<AddSectionCommand, SectionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public AddSectionCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<SectionDto> Handle(
        AddSectionCommand request, CancellationToken ct)
    {
        await CurriculumGuard.GetCourseAndValidateOwnershipAsync(
            request.CourseId, _uow, _currentUser, ct);

        // Auto-assign order
        var existingSections = await _uow.Repository<Section>()
            .GetAllWithSpecAsync(
                new SectionsByCourseSpec(request.CourseId), ct);

        var nextOrder = existingSections.Any()
            ? existingSections.Max(s => s.Order) + 1
            : 1;

        var section = new Section
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Order = nextOrder
        };

        await _uow.Repository<Section>().AddAsync(section, ct);
        await _uow.CompleteAsync(ct);

        return _mapper.Map<SectionDto>(section);
    }
}