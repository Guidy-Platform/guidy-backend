namespace CoursePlatform.Application.Features.Reviews.DTOs;

public class ReviewSummaryDto
{
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int FiveStars { get; set; }
    public int FourStars { get; set; }
    public int ThreeStars { get; set; }
    public int TwoStars { get; set; }
    public int OneStar { get; set; }

    // Percentages for progress bars in the UI

    public double FiveStarsPercent => TotalRatings > 0
        ? Math.Round((double)FiveStars / TotalRatings * 100, 1) : 0;
    public double FourStarsPercent => TotalRatings > 0
        ? Math.Round((double)FourStars / TotalRatings * 100, 1) : 0;
    public double ThreeStarsPercent => TotalRatings > 0
        ? Math.Round((double)ThreeStars / TotalRatings * 100, 1) : 0;
    public double TwoStarsPercent => TotalRatings > 0
        ? Math.Round((double)TwoStars / TotalRatings * 100, 1) : 0;
    public double OneStarPercent => TotalRatings > 0
        ? Math.Round((double)OneStar / TotalRatings * 100, 1) : 0;
}