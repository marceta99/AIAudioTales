using AutoMapper;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Web.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, DTOReturnUser>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
               ;
        }
    }
}
