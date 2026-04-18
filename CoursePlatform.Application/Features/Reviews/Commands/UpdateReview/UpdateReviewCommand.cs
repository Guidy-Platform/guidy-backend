using CoursePlatform.Application.Features.Reviews.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(
    int ReviewId,
    int Rating,
    string Comment
) : IRequest<ReviewDto>;