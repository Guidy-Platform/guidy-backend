using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Wishlist.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler
    : IRequestHandler<RemoveFromWishlistCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromWishlistCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        RemoveFromWishlistCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new WishlistItemByStudentAndCourseSpec(
            studentId, request.CourseId);

        var item = await _uow.Repository<WishlistItem>()
                             .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException(
                "Course not found in your wishlist.");

        _uow.Repository<WishlistItem>().Delete(item);
        await _uow.CompleteAsync(ct);

        return Unit.Value;
    }
}