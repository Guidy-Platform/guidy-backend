using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Notifications.Specifications;

public class NotificationByIdSpec : BaseSpecification<Notification>
{
    public NotificationByIdSpec(int id, Guid userId)
        : base(n => n.Id == id && n.UserId == userId)
    { }
}