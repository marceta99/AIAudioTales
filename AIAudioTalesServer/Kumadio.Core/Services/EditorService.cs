using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Infrastructure.Repositories;
using AIAudioTalesServer.Web.DTOS;
using AutoMapper;
using Kumadio.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Kumadio.Core.Services
{
    public class EditorService : IEditorService
    {
        private readonly IEditorRepository _editorRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;  // if you want caching at the service level

        public EditorService(
            IEditorRepository editorRepository,
            ICatalogRepository catalogRepository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _editorRepository = editorRepository;
            _catalogRepository = catalogRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<int> AddBookAsync(DTOCreateBook dto, int creatorId)
        {
            var domainBook = new Book
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                CreatorId = creatorId
            };
            var created = await _editorRepository.AddBookAsync(domainBook);
            return created.Id;
        }

        public async Task<DTOReturnPart?> AddRootPartAsync(DTOCreateRootPart root, HttpRequest request)
        {
            // 1) Check if book exists, etc. The repository method is pure DB, so we do logic here
            var domainBook = await _catalogRepository.GetBookByIdAsync(root.BookId);
            if (domainBook == null) return null;

            // 2) Upload audio
            var audioLink = await UploadAsync(root.PartAudio, request);
            if (audioLink == null) return null;

            // 3) Create BookPart
            var bookPart = new BookPart
            {
                BookId = root.BookId,
                IsRoot = true,
                PartAudioLink = audioLink
            };
            var createdPart = await _editorRepository.AddBookPartAsync(bookPart);

            // 4) Add answers
            var answersToAdd = new List<Answer>();
            if (root.Answers != null)
            {
                foreach (var a in root.Answers)
                {
                    answersToAdd.Add(new Answer
                    {
                        Text = a.Text,
                        CurrentPartId = createdPart.Id
                    });
                }
                await _editorRepository.AddAnswersAsync(answersToAdd);
                createdPart.Answers = answersToAdd;
            }
            return _mapper.Map<DTOReturnPart>(createdPart);
        }

        public async Task<DTOReturnPart?> AddBookPartAsync(DTOCreatePart part, HttpRequest request)
        {
            // 1) Check if book exists
            var domainBook = await _catalogRepository.GetBookByIdAsync(part.BookId);
            if (domainBook == null) return null;

            // 2) Upload audio
            var audioLink = await UploadAsync(part.PartAudio, request);
            if (audioLink == null) return null;

            // 3) Create child BookPart
            var bookPart = new BookPart
            {
                BookId = part.BookId,
                PartAudioLink = audioLink
            };
            var createdPart = await _editorRepository.AddBookPartAsync(bookPart);

            // 4) Link it to parent answer
            // Note that we do not store the “parentAnswer.NextPartId” here
            var parentAnswer = await _catalogRepository.GetAnswersForPartAsync(part.ParentAnswerId);
            // Simplify checking, or you can do a dedicated method to fetch a single Answer by ID
            var theParentAnswer = parentAnswer.FirstOrDefault(a => a.Id == part.ParentAnswerId);
            if (theParentAnswer != null && theParentAnswer.NextPartId == null)
            {
                theParentAnswer.NextPartId = createdPart.Id;
                await _editorRepository.UpdateAnswerAsync(theParentAnswer);
                // (We’d need the dbContext or a “repository.UpdateAnswer()” method.)
            }

            // 5) Add child answers if any
            var answersToAdd = new List<Answer>();
            if (part.Answers != null)
            {
                foreach (var ans in part.Answers)
                {
                    if (!string.IsNullOrEmpty(ans.Text))
                    {
                        answersToAdd.Add(new Answer
                        {
                            Text = ans.Text,
                            CurrentPartId = createdPart.Id
                        });
                    }
                }
                await _editorRepository.AddAnswersAsync(answersToAdd);
                createdPart.Answers = answersToAdd;
            }

            return _mapper.Map<DTOReturnPart>(createdPart);
        }


        public async Task<string?> UploadAsync(IFormFile file, HttpRequest request)
        {
            // e.g. validate file size/extension here
            if (file == null || file.Length == 0) return null;

            // We'll do a basic approach: generate a link
            // For a real scenario, you'd store them in some folder or an S3 bucket, etc.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            // Return the URL
            var audioLink = $"https://{request.Host}/uploads/{fileName}";
            // You can do the actual disk I/O here or in the repository if you prefer “pure data”.
            // For pure DB approach, you could store the file in DB as bytes (less common).
            return audioLink;
        }

    }
}
