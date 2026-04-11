// Application/Features/UserProfile/Queries/GetInstructorProfile/GetInstructorProfileQueryHandler.cs
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Features.UserProfile.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Application.Features.UserProfile.Queries.GetInstructorProfile;

public class GetInstructorProfileQueryHandler
    : IRequestHandler<GetInstructorProfileQuery, InstructorProfileDto>
{
    private readonly UserManager<AppUser> _userManager;

    public GetInstructorProfileQueryHandler(UserManager<AppUser> userManager)
        => _userManager = userManager;

    public async Task<InstructorProfileDto> Handle(
        GetInstructorProfileQuery request, CancellationToken ct)
    {
        
        var user = await _userManager.FindByNameAsync(request.Username)
            ?? throw new NotFoundException(
                $"Instructor '{request.Username}' was not found.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Instructor"))
            throw new NotFoundException(
                $"Instructor '{request.Username}' was not found.");

        return new InstructorProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            TotalCourses = 0,
            TotalStudents = 0,
            AverageRating = 0
        };
    }
}