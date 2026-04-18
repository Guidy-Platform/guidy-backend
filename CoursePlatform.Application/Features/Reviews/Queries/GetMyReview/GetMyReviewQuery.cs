using CoursePlatform.Application.Features.Reviews.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Queries.GetMyReview;

public record GetMyReviewQuery(int CourseId) : IRequest<ReviewDto?>;