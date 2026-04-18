using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Reviews.Commands.CreateReview;
using CoursePlatform.Application.Features.Reviews.DTOs;
using CoursePlatform.Application.Features.Reviews.Helpers;
using CoursePlatform.Application.Features.Reviews.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler
    : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateReviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(
        UpdateReviewCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();
        // fetch the review to ensure it exists and belongs to the student
        var spec = new ReviewByIdSpec(request.ReviewId);
        var review = await _uow.Repository<Review>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Review", request.ReviewId);

        // check ownership
        if (review.StudentId != studentId)
            throw new ForbiddenException(
                "You can only update your own reviews.");

        // 3.update the review
        review.Rating = request.Rating;
        review.Comment = request.Comment;
        _uow.Repository<Review>().Update(review);

        await RatingCalculator.RecalculateAndUpdateAsync(
            review.CourseId, _uow, ct);

        await _uow.CompleteAsync(ct);

        return CreateReviewCommandHandler.MapToDto(review);
    }
}