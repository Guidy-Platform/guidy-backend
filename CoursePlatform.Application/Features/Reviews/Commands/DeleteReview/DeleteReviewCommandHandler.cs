using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Reviews.Helpers;
using CoursePlatform.Application.Features.Reviews.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteReviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        DeleteReviewCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var isAdmin = _currentUser.Roles.Contains("Admin");

        // 1. fetch Review
        var spec = new ReviewByIdSpec(request.ReviewId);
        var review = await _uow.Repository<Review>()
                               .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Review", request.ReviewId);

        // 2. check Permission
        // Admin can delete any review — Student can only delete their own
        if (!isAdmin && review.StudentId != userId)
            throw new ForbiddenException(
                "You can only delete your own reviews.");

        var courseId = review.CourseId;

        // 3 delete Review
        _uow.Repository<Review>().Delete(review);

        // recalculate the avarage 
        await RatingCalculator.RecalculateAndUpdateAsync(
            courseId, _uow, ct);

        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}