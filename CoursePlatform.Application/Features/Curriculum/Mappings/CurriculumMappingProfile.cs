using AutoMapper;
using CoursePlatform.Application.Features.Curriculum.DTOs;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Features.Curriculum.Mappings;

public class CurriculumMappingProfile : Profile
{
    public CurriculumMappingProfile()
    {
        CreateMap<Lesson, LessonDto>()
            .ForMember(d => d.Type,
                o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.ResourceCount,
                o => o.MapFrom(s => s.Resources.Count));

        CreateMap<Section, SectionDto>()
            .ForMember(d => d.LessonCount,
                o => o.MapFrom(s => s.Lessons.Count))
            .ForMember(d => d.TotalSeconds,
                o => o.MapFrom(s =>
                    s.Lessons.Sum(l => l.DurationInSeconds)));
    }
}