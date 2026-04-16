using CoursePlatform.Application.Features.Orders.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.ApplyCoupon;

public record ApplyCouponCommand(
    int OrderId,
    string CouponCode
) : IRequest<OrderDto>;