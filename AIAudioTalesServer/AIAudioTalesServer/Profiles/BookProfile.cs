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
            CreateMap<Answer, DTOReturnAnswer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.CurrentPartId, opt => opt.MapFrom(src => src.CurrentPartId))
                .ForMember(dest => dest.NextPartId, opt => opt.MapFrom(src => src.NextPartId))
                ;

            CreateMap<BasketItem, DTOReturnBasketItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.ItemPrice))
                ;

            CreateMap<BookPart, DTOReturnPart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.IsRoot, opt => opt.MapFrom(src => src.IsRoot))
                .ForMember(dest => dest.ParentAnwserId, opt => opt.MapFrom(src => src.ParentAnswer.Id))
                .ForMember(dest => dest.ParentAnswerText, opt => opt.MapFrom(src => src.ParentAnswer.Text))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers))
                .ForMember(dest => dest.PlayingPosition, opt => opt.MapFrom(src => src.PlayingPosition))
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
