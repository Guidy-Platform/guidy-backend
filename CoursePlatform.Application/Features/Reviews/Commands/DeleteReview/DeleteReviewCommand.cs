using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(int ReviewId) : IRequest<Unit>;