using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Repositories
{
    public class WorkflowRepository : GenericRepository<Workflow>, IWorkflowRepository
    {
        public WorkflowRepository(ApplicationDbContext ctx) : base(ctx) { }

        public Task<List<Workflow>> GetAllWithCreatorAsync() =>
            _dbSet
                .Include(w => w.CreatedByAdmin)               
                .AsNoTracking()
                .ToListAsync();

        public Task<Workflow?> GetByIdWithCreatorAndAdminsAsync(int id) =>
            _dbSet
                .Include(w => w.CreatedByAdmin)               
                .Include(w => w.WorkflowAdmins)               
                    .ThenInclude(wa => wa.Admin)              
                .FirstOrDefaultAsync(w => w.WorkflowId == id);
    }
}
