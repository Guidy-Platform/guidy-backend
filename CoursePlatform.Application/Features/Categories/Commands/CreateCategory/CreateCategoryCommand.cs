using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    string? IconUrl
   
) : IRequest<CategoryDto>;