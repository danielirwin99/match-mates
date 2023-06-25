using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    // Profile deriving from AutoMapper package
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // In angle brackets we want to specify where we want to go from AND then to
            CreateMap<AppUser, MemberDTO>()
            // Select the individual member and the destination
            // Then we specify the options and what we want to do --> We want to map from the source
            // --> This then gives us access to our AppUser properties which we want Photos
            // FirstOrDefault returns the first element which in this case is IsMain.Url
            .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));


            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDTO, AppUser>();
        }
    }
}