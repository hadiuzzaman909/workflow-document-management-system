using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Document>> GetDocumentsByWorkflowIdAsync(int workflowId)
        {
            return await _dbSet
                .Where(d => d.WorkflowId == workflowId && !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Name == name && !d.IsDeleted);
        }
    }
}
