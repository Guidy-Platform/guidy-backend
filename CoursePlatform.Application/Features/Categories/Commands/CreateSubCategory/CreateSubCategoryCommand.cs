using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.CreateSubCategory;

public record CreateSubCategoryCommand(
    int CategoryId,
    string Name,
    string? Description
) : IRequest<SubCategoryDto>;