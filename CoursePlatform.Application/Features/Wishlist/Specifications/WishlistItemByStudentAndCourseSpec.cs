using CoursePlatform.Application.Specifications;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Wishlist.Specifications;

public class WishlistItemByStudentAndCourseSpec : BaseSpecification<WishlistItem>
{
    public WishlistItemByStudentAndCourseSpec(Guid studentId, int courseId)
        : base(w => w.StudentId == studentId && w.CourseId == courseId)
    { }
}