using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<Unit>;