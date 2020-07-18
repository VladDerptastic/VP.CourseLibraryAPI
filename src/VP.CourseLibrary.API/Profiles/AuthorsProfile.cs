using AutoMapper;
using CourseLibrary.API.Entities;
using VP.CourseLibrary.API.Helpers;
using VP.CourseLibrary.API.DtoModels;

namespace VP.CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
                )
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge())
                );
        }
    }
}
