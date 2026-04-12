using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;