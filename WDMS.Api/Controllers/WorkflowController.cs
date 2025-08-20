using Microsoft.AspNetCore.Mvc;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;

namespace WDMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public WorkflowController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Workflow>>> GetWorkflows()
        {
            var workflows = await _workflowService.GetAllWorkflowsAsync();
            return Ok(workflows);
        }

        [HttpPost]
        public async Task<ActionResult> CreateWorkflow([FromBody] Workflow workflow)
        {
            if (workflow == null)
            {
                return BadRequest(new { message = "Workflow request body is null." });
            }

            await _workflowService.CreateWorkflowAsync(workflow);
            return CreatedAtAction(nameof(GetWorkflows), new { id = workflow.WorkflowId }, workflow);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWorkflow(int id, [FromBody] Workflow workflow)
        {
            if (id != workflow.WorkflowId)
            {
                return BadRequest(new { message = "Workflow ID mismatch." });
            }

            await _workflowService.UpdateWorkflowAsync(workflow);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkflow(int id)
        {
            await _workflowService.DeleteWorkflowAsync(id);
            return NoContent();
        }

        [HttpPost("approve-task/{taskAssignmentId}")]
        public async Task<ActionResult> ApproveTask(int taskAssignmentId)
        {
            try
            {
                await _workflowService.ApproveTaskAsync(taskAssignmentId);
                return Ok(new { message = "Task approved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving the task.", error = ex.Message });
            }
        }

        [HttpPost("reject-task/{taskAssignmentId}")]
        public async Task<ActionResult> RejectTask(int taskAssignmentId)
        {
            try
            {
                await _workflowService.RejectTaskAsync(taskAssignmentId);
                return Ok(new { message = "Task rejected successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting the task.", error = ex.Message });
            }
        }
    }
}