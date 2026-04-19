using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Notifications.Specifications;

public class MyNotificationsSpec : BaseSpecification<Notification>
{
    public MyNotificationsSpec(Guid userId, bool? unreadOnly = null)
        : base(n =>
            n.UserId == userId &&
            (!unreadOnly.HasValue || n.IsRead == !unreadOnly.Value))
    {
        AddOrderByDesc(n => n.CreatedAt);
        ApplyNoTracking();
    }
}