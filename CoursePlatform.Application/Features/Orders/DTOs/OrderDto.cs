using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Orders.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
    public string? CouponCode { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<OrderItemDto> Items { get; set; } = [];
}