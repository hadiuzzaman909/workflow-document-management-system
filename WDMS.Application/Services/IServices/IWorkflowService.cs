using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Application.Services.IServices
{
    public interface IWorkflowService
    {
        Task<List<Workflow>> GetAllWorkflowsAsync();
        Task<Workflow> GetWorkflowByIdAsync(int id);
        Task CreateWorkflowAsync(Workflow workflow);
        Task UpdateWorkflowAsync(Workflow workflow);
        Task DeleteWorkflowAsync(int id);
    }
}
