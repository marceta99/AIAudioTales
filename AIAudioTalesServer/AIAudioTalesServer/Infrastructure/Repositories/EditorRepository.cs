using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Infrastructure.Data;
using AIAudioTalesServer.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Infrastructure.Repositories
{
    public class EditorRepository : IEditorRepository
    {
        private readonly AppDbContext _dbContext;
        public EditorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IList<Answer>> AddAnswersAsync(IList<Answer> answers)
        {
            await _dbContext.Answers.AddRangeAsync(answers);
            await _dbContext.SaveChangesAsync();
            return answers;
        }
        public async Task<Book> AddBookAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<BookPart> AddBookPartAsync(BookPart bookPart)
        {
            await _dbContext.BookParts.AddAsync(bookPart);
            await _dbContext.SaveChangesAsync();
            return bookPart;
        }
        public async Task UpdateAnswerAsync(Answer answer)
        {
            _dbContext.Answers.Update(answer);
            await _dbContext.SaveChangesAsync();
        }

    }
}
