using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AutoMapper;

namespace AIAudioTalesServer.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookCreateDTO, Book>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.BookCategory, opt => opt.MapFrom(src => src.BookCategory))
                ;
            /*
            CreateMap<BookCreationDTO, Book>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.BookCategory, opt => opt.MapFrom(src => src.BookCategory))
                ;*/
            CreateMap<Book, BookReturnDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.BookCategory, opt => opt.MapFrom(src => src.BookCategory))
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
                ;
        }
    }
}
