using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.UpdateSubCategory;

public record UpdateSubCategoryCommand(
    int Id,
    int CategoryId,
    string Name,
    string? Description
) : IRequest<SubCategoryDto>;