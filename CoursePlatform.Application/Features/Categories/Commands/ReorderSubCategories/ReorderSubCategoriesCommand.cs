using CoursePlatform.Application.Features.Categories.Commands.ReorderCategories;
using MediatR;

public record ReorderSubCategoriesCommand(
    int CategoryId,
    IList<CategoryOrderItem> Items
) : IRequest<Unit>;