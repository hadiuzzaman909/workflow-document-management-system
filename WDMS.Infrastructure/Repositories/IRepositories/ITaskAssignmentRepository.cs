using WDMS.Domain.Entities;

namespace WDMS.Infrastructure.Repositories.IRepositories
{
    public interface ITaskAssignmentRepository : IGenericRepository<TaskAssignment>
    {
        Task<List<TaskAssignment>> GetAllByWorkflowIdAsync(int workflowId);
        Task<List<TaskAssignment>> GetAllByAdminIdAsync(int adminId);
    }
}

