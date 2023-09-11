using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Migrations;

namespace AIAudioTalesServer.Profiles
{
    public class StoryProfile : Profile
    {
        public StoryProfile()
        {
            CreateMap<StoryCreationDTO, Story>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                ;
            CreateMap<Story, StoryReturnDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.AudioDataUrl, opt => opt.MapFrom(src => $"data:audio/mpeg;base64,{Convert.ToBase64String(src.AudioData)}"))
                ;
        }
    }
}
