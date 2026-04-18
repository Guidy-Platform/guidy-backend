namespace CoursePlatform.Application.Features.Reviews.DTOs;

public class CourseReviewsDto
{
    public ReviewSummaryDto Summary { get; set; } = new();
    public IReadOnlyList<ReviewDto> Reviews { get; set; } = [];
}