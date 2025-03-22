using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Interfaces;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Services
{
    public class EditorService : IEditorService
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookRepository _bookRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IBookPartRepository _bookPartRepository;

        public EditorService(
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IBookRepository bookRepository,
            IAnswerRepository answerRepository,
            IBookPartRepository bookPartRepository
            )
        {
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _bookRepository = bookRepository;
            _answerRepository = answerRepository;
            _bookPartRepository = bookPartRepository;
        }
        public async Task<Result<Book>> AddBook(Book book)
        {
            return await _unitOfWork.ExecuteInTransaction<Book>(async () =>
            {
                return await _bookRepository.AddAndReturn(book);
            });
        }

        public async Task<Result<BookPart>> AddRootPart(RootPartModel root, string baseUrl)
        {
            var book = await _bookRepository.GetFirstWhere(b => b.Id == root.BookId);
            if (book == null) return DomainErrors.Editor.BookNotFound;

            var fileLink = await _fileStorage.SaveFile(root.AudioBytes, root.Extension, baseUrl);
            if (fileLink == null)
                return DomainErrors.Editor.SaveFileFailed;

            var result = await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var bookPart = new BookPart
                {
                    BookId = root.BookId,
                    IsRoot = true,
                    PartAudioLink = fileLink
                };

                var newPart = await _bookPartRepository.AddAndReturn(bookPart);
                await _bookPartRepository.SaveChanges();

                var answers = root.AnswersText.Select(answerText => new Answer
                {
                    Text = answerText,
                    CurrentPartId = newPart.Id
                }).ToList();

                await _answerRepository.AddRange(answers);
                await _answerRepository.SaveChanges();

                newPart.Answers = answers;
                return Result<BookPart>.Success(newPart);
            });

            // If the transaction failed, delete the audio file.
            if (result.IsFailure)
            {
                await _fileStorage.DeleteFile(fileLink);
            }

            return result;
        }

        public async Task<Result<BookPart>> AddBookPart(PartModel part, string host)
        {
            var book = await _bookRepository.GetFirstWhere(b => b.Id == part.BookId);
            if (book == null) return DomainErrors.Editor.BookNotFound;

            var fileLink = await _fileStorage.SaveFile(part.AudioBytes, part.Extension, host);
            if (fileLink == null)
                return DomainErrors.Editor.SaveFileFailed;

            var parentAnswer = await _answerRepository.GetFirstWhere(a => a.Id == part.ParentAnswerId);
            if (parentAnswer == null || parentAnswer.NextPartId != null)
                return DomainErrors.Editor.InvalidParentAnswer;

            var result = await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var bookPart = new BookPart
                {
                    BookId = part.BookId,
                    IsRoot = false,
                    PartAudioLink = fileLink
                };

                var newPart = await _bookPartRepository.AddAndReturn(bookPart);
                await _bookPartRepository.SaveChanges(); // saveChanges is called here so that new is for newly created part is generated

                parentAnswer.NextPartId = newPart.Id;

                var answers = part.AnswersText.Select(answerText => new Answer
                {
                    Text = answerText,
                    CurrentPartId = newPart.Id
                }).ToList();

                await _answerRepository.AddRange(answers);

                newPart.Answers = answers;
                return Result<BookPart>.Success(newPart);
            });

            // If the transaction failed, delete the audio file.
            if (result.IsFailure)
            {
                await _fileStorage.DeleteFile(fileLink);
            }

            return result;
        }

    }
}
