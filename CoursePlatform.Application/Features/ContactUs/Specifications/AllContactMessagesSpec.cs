using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.ContactUs.Specifications;

public class AllContactMessagesSpec : BaseSpecification<ContactMessage>
{
    public AllContactMessagesSpec(ContactMessageStatus? status = null)
        : base(m => !status.HasValue || m.Status == status.Value)
    {
        AddOrderByDesc(m => m.CreatedAt);
        ApplyNoTracking();
    }
}