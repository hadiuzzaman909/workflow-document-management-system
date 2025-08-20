using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Data;


namespace WDMS.Infrastructure.Repositories
{
    public class TaskAssignmentRepository : GenericRepository<TaskAssignment>, ITaskAssignmentRepository
    {
        public TaskAssignmentRepository(ApplicationDbContext ctx) : base(ctx) { }

        public Task<List<TaskAssignment>> GetAllByAdminIdAsync(int adminId)
            => _context.TaskAssignments
                .Include(t => t.Workflow)
                .Where(t => t.AdminId == adminId)
                .ToListAsync();

        public Task<List<TaskAssignment>> GetAllByWorkflowIdAsync(int workflowId)
            => _context.TaskAssignments
                .Where(t => t.WorkflowId == workflowId)
                .ToListAsync();

        public Task<List<TaskAssignment>> GetActivePendingByAdminAsync(int adminId)
            => _context.TaskAssignments
                .Include(t => t.Workflow)
                .Where(t => t.AdminId == adminId
                         && t.Status == TaskState.Pending
                         && t.IsActive)
                .OrderBy(t => t.AssignedAt)
                .ToListAsync();
    }
}