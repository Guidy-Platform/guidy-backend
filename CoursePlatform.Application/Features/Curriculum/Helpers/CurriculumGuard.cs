using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Features.Curriculum.Helpers;

public static class CurriculumGuard
{
    /// <summary>
    /// Validates that the course exists, is owned by the current user, and is in a modifiable state not under review .
    /// </summary>

    public static async Task<Course> GetCourseAndValidateOwnershipAsync(
        int courseId,
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var course = await uow.Repository<Course>()
                              .GetByIdAsync(courseId, ct)
            ?? throw new NotFoundException("Course", courseId);

        if (course.InstructorId != currentUser.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this course.");

        if (course.Status == CourseStatus.UnderReview)
            throw new BadRequestException(
                "Cannot modify a course that is under review.");

        if (course.Status == CourseStatus.Archived)
            throw new BadRequestException(
                "Cannot modify an archived course.");

        return course;
    }
}