using AutoMapper;
using System.ComponentModel.DataAnnotations;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;

namespace WDMS.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private static readonly HashSet<string> AllowedExt = new(StringComparer.OrdinalIgnoreCase)
            { ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg", ".gif" };

        private readonly IDocumentRepository _repo;
        private readonly IFileStorage _storage;
        private readonly IMapper _mapper;

        public DocumentService(IDocumentRepository repo, IFileStorage storage, IMapper mapper)
        {
            _repo = repo;
            _storage = storage;
            _mapper = mapper;
        }

        public async Task<DocumentResponse> CreateDocumentAsync(
            DocumentCreateRequest request, int createdByAdminId, CancellationToken ct = default)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ValidationException("A file is required.");

            var ext = Path.GetExtension(request.File.FileName);
            if (!AllowedExt.Contains(ext))
                throw new ValidationException($"Unsupported file type: {ext}");

            // Save to storage first
            string relativePath = await _storage.SaveAsync(request.File, "documents", ct);

            // Build entity
            var doc = new Document
            {
                Name = string.IsNullOrWhiteSpace(request.Name)
                    ? Path.GetFileNameWithoutExtension(request.File.FileName)
                    : request.Name.Trim(),
                DocumentTypeId = request.DocumentTypeId,
                WorkflowId = request.WorkflowId,
                FilePath = relativePath, // e.g. "uploads/documents/xxx.ext"
                OriginalFileName = Path.GetFileName(request.File.FileName),
                ContentType = request.File.ContentType ?? "application/octet-stream",
                FileSizeBytes = request.File.Length,
                CreatedByAdminId = createdByAdminId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _repo.AddAsync(doc);
            }
            catch
            {
                // DB failed: cleanup file
                _storage.Delete(relativePath);
                throw;
            }

            // Load with includes for names/emails if needed
            var saved = await (_repo as IDocumentRepository)!.GetByIdWithIncludesAsync(doc.DocumentId, ct) ?? doc;
            return _mapper.Map<DocumentResponse>(saved);
        }

        public async Task<DocumentResponse?> GetDocumentByIdAsync(int id, CancellationToken ct = default)
        {
            var doc = await (_repo as IDocumentRepository)!.GetByIdWithIncludesAsync(id, ct);
            return doc == null ? null : _mapper.Map<DocumentResponse>(doc);
        }

        public async Task<List<DocumentResponse>> GetAllDocumentsAsync(CancellationToken ct = default)
        {
            var docs = await (_repo as IDocumentRepository)!.GetAllWithIncludesAsync(ct);
            return _mapper.Map<List<DocumentResponse>>(docs);
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?> OpenAsync(int id, CancellationToken ct = default)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc == null) return null;
            return await _storage.OpenReadAsync(doc.FilePath, ct);
        }

        public async Task<bool> DeleteDocumentAsync(int id, CancellationToken ct = default)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc == null) return false;

            // Delete DB first; if it succeeds, remove the file
            var ok = await _repo.DeleteAsync(id);
            if (ok) _storage.Delete(doc.FilePath);
            return ok;
        }
    }
}