using WDMS.Domain.Entities;

namespace WDMS.Application.Services.IServices
{
        public interface IDocumentTypeService
        {
            Task<List<DocumentType>> GetAllDocumentTypesAsync();
            Task<DocumentType> GetDocumentTypeByIdAsync(int id);
            Task CreateDocumentTypeAsync(DocumentType documentType);
            Task UpdateDocumentTypeAsync(DocumentType documentType);
            Task DeleteDocumentTypeAsync(int id);
        }
    }
