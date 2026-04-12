using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    int Id,
    string Name,
    string? Description,
    string? IconUrl
) : IRequest<CategoryDto>;