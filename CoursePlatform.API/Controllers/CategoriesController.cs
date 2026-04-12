using CoursePlatform.Application.Features.Categories.Commands.CreateCategory;
using CoursePlatform.Application.Features.Categories.Commands.CreateSubCategory;
using CoursePlatform.Application.Features.Categories.Commands.DeleteCategory;
using CoursePlatform.Application.Features.Categories.Commands.DeleteSubCategory;
using CoursePlatform.Application.Features.Categories.Commands.UpdateCategory;
using CoursePlatform.Application.Features.Categories.Commands.UpdateSubCategory;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Queries.GetAllCategories;
using CoursePlatform.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
        => _sender = sender;

    // ─── Public Endpoints ─────────────────────────────────────────────────

    /// <summary>Get all categories with their subcategories.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(
        CancellationToken ct)
        => Ok(await _sender.Send(new GetAllCategoriesQuery(), ct));

    /// <summary>Get single category with subcategories.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(
        int id, CancellationToken ct)
        => Ok(await _sender.Send(new GetCategoryByIdQuery(id), ct));

    // ─── Admin: Category ──────────────────────────────────────────────────

    /// <summary>Create a new category.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update a category.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        int id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            id, request.Name, request.Description,
            request.IconUrl);

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Delete a category (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _sender.Send(new DeleteCategoryCommand(id), ct);
        return NoContent();
    }

    // ─── Admin: SubCategory ───────────────────────────────────────────────

    /// <summary>Add subcategory to a category.</summary>
    [HttpPost("{id:int}/subcategories")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SubCategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SubCategoryDto>> CreateSubCategory(
        int id,
        [FromBody] CreateSubCategoryRequest request,
        CancellationToken ct)
    {
        var command = new CreateSubCategoryCommand(
            id, request.Name, request.Description);

        var result = await _sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Update subcategory.</summary>
    [HttpPut("{id:int}/subcategories/{subId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SubCategoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SubCategoryDto>> UpdateSubCategory(
        int id, int subId,
        [FromBody] UpdateSubCategoryRequest request,
        CancellationToken ct)
    {
        var command = new UpdateSubCategoryCommand(
            subId, id, request.Name, request.Description);

        return Ok(await _sender.Send(command, ct));
    }

    /// <summary>Delete subcategory.</summary>
    [HttpDelete("{id:int}/subcategories/{subId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSubCategory(
        int id, int subId, CancellationToken ct)
    {
        await _sender.Send(new DeleteSubCategoryCommand(subId, id), ct);
        return NoContent();
    }
}

// ─── Request Models ────────────────────────────────────────────────────────
public record UpdateCategoryRequest(
    string Name,
    string? Description,
    string? IconUrl);

public record CreateSubCategoryRequest(
    string Name,
    string? Description);

public record UpdateSubCategoryRequest(
    string Name,
    string? Description);