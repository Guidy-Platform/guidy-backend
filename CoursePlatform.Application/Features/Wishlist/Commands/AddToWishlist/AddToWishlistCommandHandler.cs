using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Wishlist.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandHandler
    : IRequestHandler<AddToWishlistCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AddToWishlistCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        AddToWishlistCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // check if course exists and is published
        var course = await _uow.Repository<Course>()
                               .GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.Published)
            throw new BadRequestException(
                "Only published courses can be added to wishlist.");

        // not already in wishlist
        var existingSpec = new WishlistItemByStudentAndCourseSpec(
            studentId, request.CourseId);
        var alreadyExists = await _uow.Repository<WishlistItem>()
                                      .AnyAsync(existingSpec, ct);

        if (alreadyExists)
            throw new ConflictException(
                "This course is already in your wishlist.");

        // is enrolled already?
        var enrolledSpec = new EnrollmentByStudentAndCourseSpec(
            studentId, request.CourseId);
        var isEnrolled = await _uow.Repository<Enrollment>()
                                   .AnyAsync(enrolledSpec, ct);

        if (isEnrolled)
            throw new BadRequestException(
                "You are already enrolled in this course.");

        // add to wishlist
        var item = new WishlistItem
        {
            StudentId = studentId,
            CourseId = request.CourseId,
            AddedAt = DateTime.UtcNow
        };

        await _uow.Repository<WishlistItem>().AddAsync(item, ct);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}