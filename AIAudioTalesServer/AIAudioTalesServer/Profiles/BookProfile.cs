using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AutoMapper;

namespace AIAudioTalesServer.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, DTOReturnBook>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                ;

            CreateMap<BasketItem, DTOReturnBasketItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.ItemPrice))
                ;

            CreateMap<DTOCreateBook, Book>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

        }
    }
}
