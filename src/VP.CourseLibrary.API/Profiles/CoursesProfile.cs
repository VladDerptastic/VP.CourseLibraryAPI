using AutoMapper;
using CourseLibrary.API.Entities;
using VP.CourseLibrary.API.DtoModels;

namespace VP.CourseLibrary.API.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseForCreationDto, Course>();
            CreateMap<CourseForUpdateDto, Course>();
            CreateMap<Course, CourseForUpdateDto>();
        }
    }
}
