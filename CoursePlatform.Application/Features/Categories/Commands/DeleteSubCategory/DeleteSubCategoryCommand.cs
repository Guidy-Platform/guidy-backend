using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.DeleteSubCategory;

public record DeleteSubCategoryCommand(int Id, int CategoryId) : IRequest<Unit>;