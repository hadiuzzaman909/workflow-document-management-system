using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories.IRepositories;


namespace WDMS.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<DocumentResponse> CreateDocumentAsync(DocumentRequest request)
        {
            var document = _mapper.Map<Document>(request);
            await _documentRepository.AddAsync(document);
            return _mapper.Map<DocumentResponse>(document);
        }

        public async Task<DocumentResponse> GetDocumentByIdAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            return _mapper.Map<DocumentResponse>(document);
        }

        public async Task<List<DocumentResponse>> GetAllDocumentsAsync()
        {
            var documents = await _documentRepository.GetAllAsync();
            return _mapper.Map<List<DocumentResponse>>(documents);
        }

        public async Task DeleteDocumentAsync(int id)
        {
            await _documentRepository.DeleteAsync(id);
        }
    }
}