using CoursePlatform.Application.Features.Orders.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;