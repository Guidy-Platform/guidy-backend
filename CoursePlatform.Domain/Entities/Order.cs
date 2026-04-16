using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Order : AuditableEntity
{
    public Guid StudentId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalPrice { get; set; }   // before discount
    public decimal DiscountAmount { get; set; }   
    public decimal FinalPrice { get; set; }   // after discount
    public int? CouponId { get; set; }
    public string? CouponCode { get; set; }   // store code for reference, even if coupon is deleted later
    public string? PaymentIntentId { get; set; }   // Stripe
    public DateTime? PaidAt { get; set; }

    // Navigation
    public AppUser Student { get; set; } = null!;
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}