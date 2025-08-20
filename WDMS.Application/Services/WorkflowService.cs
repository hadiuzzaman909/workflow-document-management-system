using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Repositories;
using WDMS.Infrastructure.Repositories.IRepositories;


namespace WDMS.Application.Services
{
    public class WorkflowService : IWorkflowService
    {

        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IGenericRepository<Admin> _adminRepository;
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowService(
            IWorkflowRepository workflowRepository,
        ITaskAssignmentRepository taskAssignmentRepository,
                               IDocumentRepository documentRepository,
                               IGenericRepository<Admin> adminRepository)
        {
            _workflowRepository = workflowRepository;
            _taskAssignmentRepository = taskAssignmentRepository;
            _documentRepository = documentRepository;
            _adminRepository = adminRepository;
        }


        public async Task<Workflow> CreateWorkflowAsync(Workflow workflow)
        {
            await _workflowRepository.AddAsync(workflow);
            return workflow; 
        }

        public async Task AssignTasksToWorkflowAsync(int workflowId, List<int> adminIds)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId)
                          ?? throw new Exception("Workflow not found");

            if (workflow.Type == WorkflowType.Order)
            {
                for (int i = 0; i < adminIds.Count; i++)
                {
                    var task = new TaskAssignment
                    {
                        WorkflowId = workflowId,
                        AdminId = adminIds[i],
                        Status = TaskState.Pending,
                        StepOrder = i,
                        IsActive = (i == 0),               
                        AssignedAt = DateTime.UtcNow
                    };
                    await _taskAssignmentRepository.AddAsync(task);
                }
            }
            else
            {
                foreach (var adminId in adminIds)
                {
                    var task = new TaskAssignment
                    {
                        WorkflowId = workflowId,
                        AdminId = adminId,
                        Status = TaskState.Pending,
                        StepOrder = 0,
                        IsActive = true,                   
                        AssignedAt = DateTime.UtcNow
                    };
                    await _taskAssignmentRepository.AddAsync(task);
                }
            }
        }

        public async Task ApproveTaskAsync(int taskAssignmentId)
        {
            var task = await _taskAssignmentRepository.GetByIdAsync(taskAssignmentId)
                       ?? throw new Exception("Task not found");

            task.Status = TaskState.Approved;
            task.ApprovedAt = DateTime.UtcNow;
            task.IsActive = false;
            await _taskAssignmentRepository.UpdateAsync(task);

            var workflow = await _workflowRepository.GetByIdAsync(task.WorkflowId);
            if (workflow?.Type == WorkflowType.Order)
            {
                var all = (await _taskAssignmentRepository.GetAllByWorkflowIdAsync(task.WorkflowId))
                          .OrderBy(t => t.StepOrder).ToList();

                var currentIndex = all.FindIndex(t => t.TaskAssignmentId == taskAssignmentId);
                if (currentIndex >= 0 && currentIndex < all.Count - 1)
                {
                    var next = all[currentIndex + 1];
                    if (next.Status == TaskState.Pending && !next.IsActive)
                    {
                        next.IsActive = true;
                        next.AssignedAt = DateTime.UtcNow;
                        await _taskAssignmentRepository.UpdateAsync(next);
                    }
                }
            }

            await UpdateDocumentStatus(task.WorkflowId);
        }

        public async Task RejectTaskAsync(int taskAssignmentId)
        {
            var task = await _taskAssignmentRepository.GetByIdAsync(taskAssignmentId)
                       ?? throw new Exception("Task not found");

            task.Status = TaskState.Rejected;
            task.RejectedAt = DateTime.UtcNow;
            task.IsActive = false;
            await _taskAssignmentRepository.UpdateAsync(task);

            var workflow = await _workflowRepository.GetByIdAsync(task.WorkflowId);
            if (workflow?.Type == WorkflowType.Order)
            {
                var rest = await _taskAssignmentRepository.GetAllByWorkflowIdAsync(task.WorkflowId);
                foreach (var t in rest.Where(t => t.TaskAssignmentId != taskAssignmentId && t.Status == TaskState.Pending))
                {
                    if (t.IsActive)
                    {
                        t.IsActive = false;
                        await _taskAssignmentRepository.UpdateAsync(t);
                    }
                }
            }

            await UpdateDocumentStatus(task.WorkflowId);
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
