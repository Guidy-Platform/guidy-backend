using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Orders.DTOs;
using CoursePlatform.Application.Features.Orders.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using MediatR;

namespace CoursePlatform.Application.Features.Orders.Commands.ApplyCoupon;

public class ApplyCouponCommandHandler
    : IRequestHandler<ApplyCouponCommand, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _payment;

    public ApplyCouponCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IPaymentService payment)
    {
        _uow = uow;
        _currentUser = currentUser;
        _payment = payment;
    }

    public async Task<OrderDto> Handle(
        ApplyCouponCommand request, CancellationToken ct)
    {
        var studentId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        // 1. جيب الـ Order
        var spec = new OrderByIdSpec(request.OrderId);
        var order = await _uow.Repository<Order>()
                              .GetEntityWithSpecAsync(spec, ct)
            ?? throw new NotFoundException("Order", request.OrderId);

        if (order.StudentId != studentId)
            throw new ForbiddenException();

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException(
                "Cannot apply coupon to a non-pending order.");

        // 2. تحقق من الـ Coupon
        var couponSpec = new CouponByCodeSpec(request.CouponCode);
        var coupon = await _uow.Repository<Coupon>()
                                   .GetEntityWithSpecAsync(couponSpec, ct)
            ?? throw new NotFoundException(
                $"Coupon '{request.CouponCode}' not found.");

        if (!coupon.CanBeUsed)
            throw new BadRequestException(
                coupon.IsExpired ? "This coupon has expired." :
                coupon.IsUsageLimitReached ? "This coupon has reached its usage limit." :
                                               "This coupon is no longer active.");

        // calculate discount based on type
        ;
        var discount = coupon.CalculateDiscount(order.TotalPrice);

        var newFinalPrice = Math.Max(0, order.TotalPrice - discount);

        // update coupon details in order
        order.CouponId = coupon.Id;
        order.CouponCode = coupon.Code;
        order.DiscountAmount = discount;
        order.FinalPrice = newFinalPrice;

        // update payment intent only if there's a change in price and there's an existing payment intent
        if (order.PaymentIntentId is not null && newFinalPrice > 0)
        {
            await _payment.UpdatePaymentIntentAsync(
                order.PaymentIntentId, newFinalPrice, ct);
        }

        // 6  usage count+1 
        coupon.UsedCount++;
        _uow.Repository<Coupon>().Update(coupon);
        _uow.Repository<Order>().Update(order);
        await _uow.CompleteAsync(ct);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order o) => new()
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
    };
}