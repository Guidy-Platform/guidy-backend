// Application/Features/UserProfile/Queries/GetInstructorProfile/GetInstructorProfileQuery.cs
using CoursePlatform.Application.Features.UserProfile.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.UserProfile.Queries.GetInstructorProfile;

public record GetInstructorProfileQuery(string Username)  
    : IRequest<InstructorProfileDto>;