using WDMS.Domain.Entities;

namespace WDMS.Infrastructure.Repositories.IRepositories
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {

        Task<List<Document>> GetDocumentsByWorkflowIdAsync(int workflowId);
        Task<Document?> GetDocumentByNameAsync(string name);
    }
}

