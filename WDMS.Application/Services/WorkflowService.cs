
using WDMS.Application.Services.IServices;
using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Services
    {
        public class WorkflowService : IWorkflowService
        {
            private readonly IGenericRepository<Workflow> _workflowRepository;

            public WorkflowService(IGenericRepository<Workflow> workflowRepository)
            {
                _workflowRepository = workflowRepository;
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
