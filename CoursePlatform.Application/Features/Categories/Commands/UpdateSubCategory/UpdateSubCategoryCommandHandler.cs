using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.UpdateSubCategory;

public class UpdateSubCategoryCommandHandler
    : IRequestHandler<UpdateSubCategoryCommand, SubCategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public UpdateSubCategoryCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<SubCategoryDto> Handle(
        UpdateSubCategoryCommand request, CancellationToken ct)
    {
        var subCategory = await _uow.Repository<SubCategory>()
                                    .GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("SubCategory", request.Id);

        // التحقق إن الـ SubCategory تابعة للـ Category دي
        if (subCategory.CategoryId != request.CategoryId)
            throw new ForbiddenException(
                "This subcategory does not belong to the specified category.");

        subCategory.Name = request.Name;
        subCategory.Slug = SlugHelper.GenerateSlug(request.Name);
        subCategory.Description = request.Description;

        _uow.Repository<SubCategory>().Update(subCategory);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);
        await _cache.RemoveAsync($"categories:{request.CategoryId}", ct);

        return _mapper.Map<SubCategoryDto>(subCategory);
    }
}