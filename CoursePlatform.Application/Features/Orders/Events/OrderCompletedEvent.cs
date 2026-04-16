namespace CoursePlatform.Application.Features.Orders.Events;

public class OrderCompletedEvent
{
    public int OrderId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentEmail { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal FinalPrice { get; set; }
    public List<int> CourseIds { get; set; } = [];
    public List<string> CourseTitles { get; set; } = [];
}