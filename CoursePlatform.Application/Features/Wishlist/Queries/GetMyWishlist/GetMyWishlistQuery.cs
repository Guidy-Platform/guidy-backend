using CoursePlatform.Application.Features.Wishlist.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Wishlist.Queries.GetMyWishlist;

public record GetMyWishlistQuery : IRequest<IReadOnlyList<WishlistItemDto>>;