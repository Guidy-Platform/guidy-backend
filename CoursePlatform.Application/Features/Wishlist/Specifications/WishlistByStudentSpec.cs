using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Wishlist.Specifications;

public class WishlistByStudentSpec : BaseSpecification<WishlistItem>
{
    public WishlistByStudentSpec(Guid studentId)
        : base(w => w.StudentId == studentId)
    {
        AddInclude(w => w.Course);
        AddInclude("Course.Instructor");
        AddOrderByDesc(w => w.AddedAt);
        ApplyNoTracking();
    }
}