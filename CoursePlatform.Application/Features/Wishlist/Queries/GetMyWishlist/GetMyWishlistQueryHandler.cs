using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Enrollments.Specifications;
using CoursePlatform.Application.Features.Wishlist.DTOs;
using CoursePlatform.Application.Features.Wishlist.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Queries.GetMyWishlist;

public class GetMyWishlistQueryHandler
    : IRequestHandler<GetMyWishlistQuery, IReadOnlyList<WishlistItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyWishlistQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<WishlistItemDto>> Handle(
        GetMyWishlistQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new WishlistByStudentSpec(studentId);
        var items = await _uow.Repository<WishlistItem>()
                               .GetAllWithSpecAsync(spec, ct);

        var result = new List<WishlistItemDto>();

        foreach (var item in items)
        {
            // check if the student enrolled after adding to wishlist
            var enrolledSpec = new EnrollmentByStudentAndCourseSpec(
                studentId, item.CourseId);
            var isEnrolled = await _uow.Repository<Enrollment>()
                                       .AnyAsync(enrolledSpec, ct);

            result.Add(new WishlistItemDto
            {
                Id = item.Id,
                CourseId = item.CourseId,
                CourseTitle = item.Course.Title,
                ThumbnailUrl = item.Course.ThumbnailUrl,
                InstructorName = item.Course.Instructor?.FullName
                                 ?? string.Empty,
                Price = item.Course.Price,
                DiscountPrice = item.Course.DiscountPrice,
                AverageRating = item.Course.AverageRating,
                TotalRatings = item.Course.TotalRatings,
                Level = item.Course.Level.ToString(),
                AddedAt = item.AddedAt,
                IsEnrolled = isEnrolled
            });
        }

        return result;
    }
}