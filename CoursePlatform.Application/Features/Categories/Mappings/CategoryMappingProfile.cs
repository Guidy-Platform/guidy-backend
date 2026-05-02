using AutoMapper;
using CoursePlatform.Application.Features.Categories.DTOs;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Categories.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<SubCategory, SubCategoryDto>()
            .ForMember(d => d.CourseCount,
                o => o.MapFrom(s =>
                    s.Courses.Count(c =>
                        !c.IsDeleted &&
                        c.Status == Domain.Enums.CourseStatus.Published)));

        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.TotalCourses,
                o => o.MapFrom(s =>           
                    s.SubCategories
                     .SelectMany(sub => sub.Courses)
                     .Count(c => !c.IsDeleted &&
                                 c.Status == Domain.Enums.CourseStatus.Published)));

        CreateMap<Category, CategorySummaryDto>()
            .ForMember(d => d.SubCategoryCount,
                o => o.MapFrom(s => s.SubCategories.Count))
            .ForMember(d => d.TotalCourses,
                o => o.MapFrom(s =>         
                    s.SubCategories
                     .SelectMany(sub => sub.Courses)
                     .Count(c => !c.IsDeleted &&
                                 c.Status == Domain.Enums.CourseStatus.Published)));
    }
}