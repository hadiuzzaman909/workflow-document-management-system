using Microsoft.AspNetCore.Http;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;

namespace WDMS.Infrastructure.Services
{
    public interface IDocumentService
    {
        Task<DocumentResponse> CreateDocumentAsync(DocumentRequest request);
        Task<DocumentResponse> GetDocumentByIdAsync(int id);
        Task<List<DocumentResponse>> GetAllDocumentsAsync();
        Task DeleteDocumentAsync(int id);
    }
}
