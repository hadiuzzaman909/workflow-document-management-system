using WDMS.Domain.Entities;
using WDMS.Infrastructure.Repositories.IRepositories;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<List<Document>> GetAllWithIncludesAsync(CancellationToken ct = default);
    Task<Document?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default);

    Task<List<Document>> GetDocumentsByWorkflowIdAsync(int workflowId, CancellationToken ct = default);
}

