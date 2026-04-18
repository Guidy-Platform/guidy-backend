using CoursePlatform.Application.Features.Reviews.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Queries.GetCourseReviews;

public record GetCourseReviewsQuery(int CourseId)
    : IRequest<CourseReviewsDto>;