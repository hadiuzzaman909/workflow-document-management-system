using Microsoft.AspNetCore.Http;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;

namespace WDMS.Application.Services
{
    public interface IDocumentService
    {
        Task<DocumentResponse> CreateDocumentAsync(
            DocumentCreateRequest request,
            int createdByAdminId,
            CancellationToken ct = default);

        Task<DocumentResponse?> GetDocumentByIdAsync(int id, CancellationToken ct = default);
        Task<List<DocumentResponse>> GetAllDocumentsAsync(CancellationToken ct = default);

        Task<(Stream Stream, string ContentType, string FileName)?> OpenAsync(int id, CancellationToken ct = default);

        Task<bool> DeleteDocumentAsync(int id, CancellationToken ct = default);
    }
}
