using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Repositories
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskAssignment>> GetAllAsync()
        {
            return await _context.TaskAssignments
                .Include(t => t.Admin)
                .Include(t => t.Workflow)
                .ToListAsync();
        }

        public async Task<TaskAssignment?> GetByIdAsync(int id)
        {
            return await _context.TaskAssignments
                .Include(t => t.Admin)
                .Include(t => t.Workflow)
                .FirstOrDefaultAsync(t => t.TaskAssignmentId == id);
        }

        public async Task<TaskAssignment> AddAsync(TaskAssignment entity)
        {
            await _context.TaskAssignments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TaskAssignment> UpdateAsync(TaskAssignment entity)
        {
            _context.TaskAssignments.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            if (task != null)
            {
                _context.TaskAssignments.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<TaskAssignment>> GetAllByWorkflowIdAsync(int workflowId)
        {
            return await _context.TaskAssignments
                .Include(t => t.Admin)
                .Include(t => t.Workflow)
                .Where(t => t.WorkflowId == workflowId)
                .ToListAsync();
        }

        public async Task<List<TaskAssignment>> GetAllByAdminIdAsync(int adminId)
        {
            return await _context.TaskAssignments
                .Include(t => t.Admin)
                .Include(t => t.Workflow)
                .Where(t => t.AdminId == adminId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var task = await _context.TaskAssignments.FindAsync(id);
            return task != null;
        }
    }
}
