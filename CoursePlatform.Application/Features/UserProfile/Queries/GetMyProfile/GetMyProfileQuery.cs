using CoursePlatform.Application.Features.UserProfile.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.UserProfile.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<UserProfileDto>;