using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Commands.AddToWishlist;

public record AddToWishlistCommand(int CourseId) : IRequest<Unit>;