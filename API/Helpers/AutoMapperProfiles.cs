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
            CreateMap<RegisterDTO, AppUser>();
            // Mapping from the Message to the MessageDTO
            CreateMap<Message, MessageDTO>()
            // Sender
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
            // Receiver
            .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));

            // Converting the Date Time to UTC Format
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            // We need to create another map for Optional DateTime
            // If we do get a value then we want to do the same as above
            CreateMap<DateTime?, DateTime?>().ConvertUsing
            (d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
        }
    }
}