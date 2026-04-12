using MediatR;

namespace CoursePlatform.Application.Features.Courses.Commands.SubmitCourseForReview;

public record SubmitCourseForReviewCommand(int CourseId) : IRequest<Unit>;