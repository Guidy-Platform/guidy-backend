using CoursePlatform.Application.Features.Categories.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;