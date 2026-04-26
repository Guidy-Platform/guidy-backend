namespace CoursePlatform.Application.Features.Wishlist.DTOs;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public string Level { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public bool IsEnrolled { get; set; } 
}