


using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Services
    {
        public class DocumentTypeService : IDocumentTypeService
        {
            private readonly IGenericRepository<DocumentType> _documentTypeRepository;

            public DocumentTypeService(IGenericRepository<DocumentType> documentTypeRepository)
            {
                _documentTypeRepository = documentTypeRepository;
            }

            public async Task<List<DocumentType>> GetAllDocumentTypesAsync()
            {
                return await _documentTypeRepository.GetAllAsync();
            }

            public async Task<DocumentType> GetDocumentTypeByIdAsync(int id)
            {
                return await _documentTypeRepository.GetByIdAsync(id);
            }

            public async Task CreateDocumentTypeAsync(DocumentType documentType)
            {
                await _documentTypeRepository.AddAsync(documentType);
            }

            public async Task UpdateDocumentTypeAsync(DocumentType documentType)
            {
                await _documentTypeRepository.UpdateAsync(documentType);
            }

            public async Task DeleteDocumentTypeAsync(int id)
            {
                await _documentTypeRepository.DeleteAsync(id);
            }
        }
    }


