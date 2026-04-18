using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Reviews.Commands.CreateReview;
using CoursePlatform.Application.Features.Reviews.DTOs;
using CoursePlatform.Application.Features.Reviews.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Queries.GetMyReview;

public class GetMyReviewQueryHandler
    : IRequestHandler<GetMyReviewQuery, ReviewDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyReviewQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto?> Handle(
        GetMyReviewQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new ReviewByStudentAndCourseSpec(
            studentId, request.CourseId);
        var review = await _uow.Repository<Review>()
                               .GetEntityWithSpecAsync(spec, ct);

        return review is null
            ? null
            : CreateReviewCommandHandler.MapToDto(review);
    }
}