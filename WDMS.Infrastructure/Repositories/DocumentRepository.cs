using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories;

public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext ctx) : base(ctx) { }

    public Task<List<Document>> GetAllWithIncludesAsync(CancellationToken ct = default) =>
        _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.Workflow)
            .Include(d => d.CreatedByAdmin)
            .AsNoTracking()
            .ToListAsync(ct);

    public Task<Document?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default) =>
        _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.Workflow)
            .Include(d => d.CreatedByAdmin)
            .FirstOrDefaultAsync(d => d.DocumentId == id, ct);

    public Task<List<Document>> GetDocumentsByWorkflowIdAsync(int workflowId, CancellationToken ct = default)
       => _context.Documents
           .Where(d => d.WorkflowId == workflowId && !d.IsDeleted)
           .ToListAsync(ct);
}
