using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using AIAudioTalesServer.Data.Interfaces;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public BooksRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<Book> AddNewBook(BookCreateDTO newBook)
        {
            var book = _mapper.Map<Book>(newBook);
            using (var memoryStream = new MemoryStream())
            {
                await newBook.ImageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                book.ImageData = imageBytes;
            }
            
            var createdBook = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return createdBook.Entity;
        }
        public async Task<IList<Book>> GetAllBooks()
        {
            var books = await _dbContext.Books.ToListAsync();
            return books;
        }

        public async Task<Book> GetBook(int id)
        {
            var book = await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
            return book;
        }



    }
}
