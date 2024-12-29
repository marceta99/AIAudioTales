using AIAudioTalesServer.Domain.Entities;

namespace AIAudioTalesServer.Infrastructure.Interfaces
{
    public interface IEditorRepository
    {
        Task<Book> AddBookAsync(Book book);
        Task<BookPart> AddBookPartAsync(BookPart bookPart);
        Task<IList<Answer>> AddAnswersAsync(IList<Answer> answers);
        Task UpdateAnswerAsync(Answer answer);
    }
}
