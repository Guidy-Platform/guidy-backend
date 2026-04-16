using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Orders.DTOs;
using CoursePlatform.Application.Features.Orders.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler
    : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyOrdersQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetMyOrdersQuery request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var spec = new MyOrdersSpec(studentId);
        var orders = await _uow.Repository<Order>()
                               .GetAllWithSpecAsync(spec, ct);

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            Status = o.Status.ToString(),
            TotalPrice = o.TotalPrice,
            DiscountAmount = o.DiscountAmount,
            FinalPrice = o.FinalPrice,
            CouponCode = o.CouponCode,
            PaidAt = o.PaidAt,
            CreatedAt = o.CreatedAt,
            Items = o.OrderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                CourseId = i.CourseId,
                CourseTitle = i.CourseTitle,
                Price = i.Price
            }).ToList()
        }).ToList();
    }
}