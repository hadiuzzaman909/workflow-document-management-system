
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Repositories
{
    public interface ITaskAssignmentRepository : IGenericRepository<TaskAssignment>
    {
        Task<List<TaskAssignment>> GetAllByAdminIdAsync(int adminId);
        Task<List<TaskAssignment>> GetAllByWorkflowIdAsync(int workflowId);


        Task<List<TaskAssignment>> GetActivePendingByAdminAsync(int adminId);
    }
}
