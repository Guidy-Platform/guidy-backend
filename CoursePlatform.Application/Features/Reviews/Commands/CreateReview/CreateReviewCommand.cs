using CoursePlatform.Application.Features.Reviews.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    int CourseId,
    int Rating,
    string Comment
) : IRequest<ReviewDto>;