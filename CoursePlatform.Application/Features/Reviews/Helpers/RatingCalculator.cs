using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Reviews.Helpers;

public static class RatingCalculator
{
    public static async Task RecalculateAndUpdateAsync(
        int courseId,
        IUnitOfWork uow,
        CancellationToken ct,
        Review? newReview = null,   
        int? removedRating = null)  
    {
        var course = await uow.Repository<Course>()
                              .GetByIdAsync(courseId, ct);
        if (course is null) return;

        var spec = new Specifications.CourseReviewsSpec(courseId);
        var reviews = await uow.Repository<Review>()
                               .GetAllWithSpecAsync(spec, ct);

        var allRatings = reviews.Select(r => r.Rating).ToList();

        if (newReview is not null)
            allRatings.Add(newReview.Rating);

        if (allRatings.Count == 0)
        {
            course.AverageRating = 0;
            course.TotalRatings = 0;
        }
        else
        {
            course.AverageRating = Math.Round(
                allRatings.Average(), 1);
            course.TotalRatings = allRatings.Count;
        }

        uow.Repository<Course>().Update(course);
    }
}