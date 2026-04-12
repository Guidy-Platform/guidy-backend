using AutoMapper;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Helpers;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Application.Features.Categories.Specifications;
using CoursePlatform.Domain.Entities;
using MediatR;

namespace CoursePlatform.Application.Features.Categories.Commands.CreateSubCategory;

public class CreateSubCategoryCommandHandler
    : IRequestHandler<CreateSubCategoryCommand, SubCategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateSubCategoryCommandHandler(
        IUnitOfWork uow,
        IMapper mapper,
        ICacheService cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }
    public async Task<SubCategoryDto> Handle(
        CreateSubCategoryCommand request, CancellationToken ct)
    {
        var category = await _uow.Repository<Category>()
                                 .GetByIdAsync(request.CategoryId, ct)
            ?? throw new NotFoundException("Category", request.CategoryId);

        // تحقق من تكرار الاسم داخل نفس الـ Category
        var nameExists = await _uow.Repository<SubCategory>()
                                   .AnyAsync(
                                       new SubCategoryBySlugInCategorySpec(
                                           request.Name, request.CategoryId), ct);
        if (nameExists)
            throw new ConflictException(
                $"A subcategory named '{request.Name}' already exists in this category.");

        // slug فريد داخل نفس الـ Category
        var slug = await SlugHelper.GenerateUniqueSlugAsync(
            request.Name,
            async s =>
            {
                var spec = new SubCategoryBySlugInCategorySpec(s, request.CategoryId);
                return await _uow.Repository<SubCategory>().AnyAsync(spec, ct);
            });
        // automatically set the order to be the next one in the category

            //var spec = new SubCategoryByCategoryIdSpec(request.CategoryId);

            //var subCategories = await _uow.Repository<SubCategory>()
            //                              .GetAllWithSpecAsync(spec, ct);

            //var nextOrder = subCategories.Any()
            //    ? subCategories.Max(sc => sc.Order) + 1
            //    : 1;

            var maxOrder = await _uow.Repository<SubCategory>()
                .GetMaxAsync(
                    sc => sc.CategoryId == request.CategoryId,
                    sc => sc.Order,
                    ct);

        var nextOrder = (maxOrder ?? 0) + 1;

        var subCategory = new SubCategory
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            Order =nextOrder,
        }; 

        await _uow.Repository<SubCategory>().AddAsync(subCategory, ct);
        await _uow.CompleteAsync(ct);

        await _cache.RemoveAsync("categories:all", ct);
        await _cache.RemoveAsync($"categories:{request.CategoryId}", ct);

        return _mapper.Map<SubCategoryDto>(subCategory);
    }
}