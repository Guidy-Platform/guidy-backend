using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public record RemoveFromWishlistCommand(int CourseId) : IRequest<Unit>;