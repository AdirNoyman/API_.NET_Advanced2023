using AutoMapper;

namespace CourseLibrary.API.Profiles;
public class CoursesProfile : Profile
{
    public CoursesProfile()
    {
        CreateMap<Entities.Course, Models.CourseDto>();
        CreateMap<Models.CourseForCreationDto, Entities.Course>();
        // Map CourseForUpdateDto (to)=> Course(DB Entity)
        // ReverseMap() - enable us to map also in other way around
        CreateMap<Models.CourseForUpdateDto, Entities.Course>().ReverseMap();
    }
}