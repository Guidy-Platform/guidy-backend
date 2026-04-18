using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Reviews.DTOs;
using CoursePlatform.Application.Features.Reviews.Helpers;
using CoursePlatform.Application.Features.Reviews.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(
        CreateReviewCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // check if course enrollment exists
        var enrollmentSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var isEnrolled = await _uow.Repository<Enrollment>()
                                   .AnyAsync(enrollmentSpec, ct);
        if (!isEnrolled)
            throw new ForbiddenException(
                "You must be enrolled in this course to write a review.");

        // 2. check if review already exists
        var existingSpec = new ReviewByStudentAndCourseSpec(
            studentId, request.CourseId);
        var exists = await _uow.Repository<Review>()
                               .AnyAsync(existingSpec, ct);
        if (exists)
            throw new ConflictException(
                "You have already reviewed this course. " +
                "Use the update endpoint to modify your review.");

        //create review
        // في CreateReviewCommandHandler.Handle()

        // 3. إنشاء الـ Review
        var review = new Review
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        await _uow.Repository<Review>().AddAsync(review, ct);

        await RatingCalculator.RecalculateAndUpdateAsync(
            request.CourseId, _uow, ct, newReview: review);  


        await _uow.CompleteAsync(ct);
        // fetch the created review with related data
        var spec = new ReviewByIdSpec(review.Id);
        var result = await _uow.Repository<Review>()
                               .GetEntityWithSpecAsync(spec, ct);

        return MapToDto(result!);
    }

    internal static ReviewDto MapToDto(Review r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        StudentName = r.Student?.FullName ?? string.Empty,
        StudentAvatar = r.Student?.ProfilePictureUrl,
        CourseId = r.CourseId,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}