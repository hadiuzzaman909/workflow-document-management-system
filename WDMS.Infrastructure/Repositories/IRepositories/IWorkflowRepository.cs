using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Entities;

namespace WDMS.Infrastructure.Repositories.IRepositories
{
    public interface IWorkflowRepository : IGenericRepository<Workflow>
    {
        Task<List<Workflow>> GetAllWithCreatorAsync();
        Task<Workflow?> GetByIdWithCreatorAndAdminsAsync(int id);
    }
}
