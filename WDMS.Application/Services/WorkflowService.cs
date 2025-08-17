using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Repositories.IRepositories;


namespace WDMS.Infrastructure.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IGenericRepository<Workflow> _workflowRepository;
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IGenericRepository<Admin> _adminRepository;

        public WorkflowService(IGenericRepository<Workflow> workflowRepository,
                               ITaskAssignmentRepository taskAssignmentRepository,
                               IDocumentRepository documentRepository,
                               IGenericRepository<Admin> adminRepository)
        {
            _workflowRepository = workflowRepository;
            _taskAssignmentRepository = taskAssignmentRepository;
            _documentRepository = documentRepository;
            _adminRepository = adminRepository;
        }

        public async Task AssignTasksToWorkflowAsync(int workflowId, List<int> adminIds)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) throw new Exception("Workflow not found");

            if (workflow.Type == WorkflowType.Order)
            {

                for (int i = 0; i < adminIds.Count; i++)
                {
                    var task = new TaskAssignment
                    {
                        WorkflowId = workflowId,
                        AdminId = adminIds[i],
                        Status = TaskState.Pending,
                        AssignedAt = DateTime.UtcNow
                    };
                    await _taskAssignmentRepository.AddAsync(task);
                }
            }
            else if (workflow.Type == WorkflowType.Pool)
            {

                foreach (var adminId in adminIds)
                {
                    var task = new TaskAssignment
                    {
                        WorkflowId = workflowId,
                        AdminId = adminId,
                        Status = TaskState.Pending,
                        AssignedAt = DateTime.UtcNow
                    };
                    await _taskAssignmentRepository.AddAsync(task);
                }
            }
        }

        public async Task ApproveTaskAsync(int taskAssignmentId)
        {
            var taskAssignment = await _taskAssignmentRepository.GetByIdAsync(taskAssignmentId);
            if (taskAssignment == null) throw new Exception("Task not found");

            taskAssignment.Status = TaskState.Approved;
            taskAssignment.ApprovedAt = DateTime.UtcNow;

            await UpdateDocumentStatus(taskAssignment.WorkflowId);
            await _taskAssignmentRepository.UpdateAsync(taskAssignment);
        }

        public async Task RejectTaskAsync(int taskAssignmentId)
        {
            var taskAssignment = await _taskAssignmentRepository.GetByIdAsync(taskAssignmentId);
            if (taskAssignment == null) throw new Exception("Task not found");

            taskAssignment.Status = TaskState.Rejected;
            taskAssignment.RejectedAt = DateTime.UtcNow;

            await UpdateDocumentStatus(taskAssignment.WorkflowId);
            await _taskAssignmentRepository.UpdateAsync(taskAssignment);
        }

        private async Task UpdateDocumentStatus(int workflowId)
        {
            var taskAssignments = await _taskAssignmentRepository.GetAllByWorkflowIdAsync(workflowId);

            var documents = await _documentRepository.GetDocumentsByWorkflowIdAsync(workflowId);


            if (documents == null || !documents.Any())
                return;

            if (taskAssignments.All(t => t.Status == TaskState.Approved))
            {
                foreach (var document in documents)
                {
                    document.State = DocumentState.Approved; 
                    await _documentRepository.UpdateAsync(document);
                }
            }
            else if (taskAssignments.Any(t => t.Status == TaskState.Rejected))
            {
                foreach (var document in documents)
                {
                    document.State = DocumentState.Rejected; 
                    await _documentRepository.UpdateAsync(document);
                }
            }
        }


        public async Task<List<TaskAssignment>> GetTasksForAdminAsync(int adminId)
        {
            var tasks = await _taskAssignmentRepository.GetAllByAdminIdAsync(adminId);
            return tasks;
        }

        public async Task<List<Workflow>> GetAllWorkflowsAsync()
        {
            return await _workflowRepository.GetAllAsync();
        }

        public async Task<Workflow> GetWorkflowByIdAsync(int id)
        {
            return await _workflowRepository.GetByIdAsync(id);
        }

        public async Task CreateWorkflowAsync(Workflow workflow)
        {
            await _workflowRepository.AddAsync(workflow);
        }

        public async Task UpdateWorkflowAsync(Workflow workflow)
        {
            await _workflowRepository.UpdateAsync(workflow);
        }

        public async Task DeleteWorkflowAsync(int id)
        {
            await _workflowRepository.DeleteAsync(id);
        }
    }
}
