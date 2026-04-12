// Application/Features/Categories/Commands/ReorderCategories/ReorderCategoriesCommand.cs
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.ReorderCategories;

public record CategoryOrderItem(int Id, int Order);

public record ReorderCategoriesCommand(
    IList<CategoryOrderItem> Items
) : IRequest<Unit>;