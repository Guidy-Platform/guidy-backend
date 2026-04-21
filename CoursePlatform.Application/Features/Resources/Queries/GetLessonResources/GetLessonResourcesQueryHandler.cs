using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Resources.DTOs;
using CoursePlatform.Application.Features.Resources.Specifications;
using CoursePlatform.Application.Features.Subscriptions.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Resources.Queries.GetLessonResources;

public class GetLessonResourcesQueryHandler
    : IRequestHandler<GetLessonResourcesQuery, IReadOnlyList<ResourceDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetLessonResourcesQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ResourceDto>> Handle(
        GetLessonResourcesQuery request, CancellationToken ct)
    {
        // جيب الـ lesson
        var lesson = await _uow.Repository<Lesson>()
                               .GetByIdAsync(request.LessonId, ct)
            ?? throw new NotFoundException("Lesson", request.LessonId);

        if (lesson.SectionId != request.SectionId)
            throw new NotFoundException("Lesson", request.LessonId);

        // Access check:
        // Instructor/Admin → يشوف كل حاجة
        // Free Preview Lesson → أي حد
        // Paid Lesson → محتاج enrollment (هيتعمل في Enrollment Module)
        var isInstructorOrAdmin =
            _currentUser.Roles.Contains("Instructor") ||
            _currentUser.Roles.Contains("Admin");

        var isSubscribed = false;
        if (_currentUser.IsAuthenticated)
        {
            var subSpec = new ActiveSubscriptionByUserSpec(
                _currentUser.UserId!.Value);
            isSubscribed = await _uow.Repository<UserSubscription>()
                                     .AnyAsync(subSpec, ct);
        }

        if (!isInstructorOrAdmin && !lesson.IsFreePreview && !isSubscribed)
        {
            // TODO: enrollment check
            if (!_currentUser.IsAuthenticated) return [];
        }

       
        var spec = new ResourcesByLessonSpec(request.LessonId);
        var resources = await _uow.Repository<Resource>()
                                  .GetAllWithSpecAsync(spec, ct);

        return resources.Select(r => new ResourceDto
        {
            Id = r.Id,
            Title = r.Title,
            FileUrl = r.FileUrl,
            FileName = r.FileName,
            FileType = r.FileType,
            FileSize = r.FileSize,
            FileSizeFormatted = r.FileSizeFormatted,
            LessonId = r.LessonId,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}