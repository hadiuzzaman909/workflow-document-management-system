using System.Collections.Generic;
using System.Threading.Tasks;
using WDMS.Domain.Entities;

namespace WDMS.Application.Services.IServices
{
    public interface IWorkflowService
    {
        Task<List<Workflow>> GetAllWorkflowsAsync();
        Task<Workflow> GetWorkflowByIdAsync(int id);
        Task<Workflow> CreateWorkflowAsync(Workflow workflow); 
        Task UpdateWorkflowAsync(Workflow workflow);
        Task RejectTaskAsync(int taskAssignmentId);
        Task ApproveTaskAsync(int taskAssignmentId);
        Task DeleteWorkflowAsync(int id);
        Task AssignTasksToWorkflowAsync(int workflowId, List<int> adminIds);
    }
}