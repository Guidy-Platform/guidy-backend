using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Features.Reviews.Commands.CreateReview;
using CoursePlatform.Application.Features.Reviews.DTOs;
using CoursePlatform.Application.Features.Reviews.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Queries.GetCourseReviews;

public class GetCourseReviewsQueryHandler
    : IRequestHandler<GetCourseReviewsQuery, CourseReviewsDto>
{
    private readonly IUnitOfWork _uow;

    public GetCourseReviewsQueryHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<CourseReviewsDto> Handle(
        GetCourseReviewsQuery request, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        var spec = new CourseReviewsSpec(request.CourseId);
        var reviews = await _uow.Repository<Review>()
                                .GetAllWithSpecAsync(spec, ct);

        var averageRating = reviews.Any()
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0;

        var summary = new ReviewSummaryDto
        {
            AverageRating = averageRating,
            TotalRatings = reviews.Count,
            FiveStars = reviews.Count(r => r.Rating == 5),
            FourStars = reviews.Count(r => r.Rating == 4),
            ThreeStars = reviews.Count(r => r.Rating == 3),
            TwoStars = reviews.Count(r => r.Rating == 2),
            OneStar = reviews.Count(r => r.Rating == 1)
        };

        return new CourseReviewsDto
        {
            Summary = summary,
            Reviews = reviews
                .Select(CreateReviewCommandHandler.MapToDto)
                .ToList()
        };
    }
}