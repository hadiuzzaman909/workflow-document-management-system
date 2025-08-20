using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WDMS.Admin.Models;
using WDMS.Application.Services.IServices;
using WDMS.Applocation.Services;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Utils;

namespace WDMS.Admin.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly IWorkflowService _workflowService;
        private readonly IAdminService _adminService;
        private readonly JwtUtils _jwtUtils;

        public WorkflowController(IWorkflowService workflowService, IAdminService adminService, JwtUtils jwtUtils)
        {
            _workflowService = workflowService;
            _adminService = adminService;
            _jwtUtils = jwtUtils;
        }

        [RequireLogin]
        [HttpGet]
        public async Task<IActionResult> Workflows()
        {
            var workflows = await _workflowService.GetAllWorkflowsAsync();
            return View(workflows);
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public async Task<IActionResult> CreateWorkflow()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            var model = new CreateWorkflowViewModel
            {
                Workflow = new Workflow(),
                Admins = admins.Select(a => new AdminListItem
                {
                    Id = a.AdminId,
                    Label = a.Email
                }).ToList()
            };
            return View(model);
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkflow(CreateWorkflowViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await HydrateAdminsAsync(model);
                return View(model);
            }

            try
            {
                var currentAdminId = GetCurrentAdminId();
                model.Workflow.CreatedByAdminId = currentAdminId;
                model.Workflow.CreatedAt = DateTime.UtcNow;
                model.Workflow.UpdatedAt = DateTime.UtcNow;

                var created = await _workflowService.CreateWorkflowAsync(model.Workflow);

                if (created != null && model.SelectedAdminIds?.Any() == true)
                {
                    await _workflowService.AssignTasksToWorkflowAsync(
                        created.WorkflowId, model.SelectedAdminIds.Distinct().ToList());
                }

                TempData["Success"] = "Workflow created successfully.";
                return RedirectToAction(nameof(Workflows));
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the workflow.";
                await HydrateAdminsAsync(model);
                return View(model);
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            try
            {
                await _workflowService.DeleteWorkflowAsync(id);
                TempData["Success"] = "Workflow deleted.";
                return RedirectToAction(nameof(Workflows));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Workflow not found.";
                return RedirectToAction(nameof(Workflows));
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the workflow.";
                return RedirectToAction(nameof(Workflows));
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public async Task<IActionResult> EditWorkflow(int id)
        {
            var wf = await _workflowService.GetWorkflowByIdAsync(id);
            if (wf == null)
            {
                TempData["Error"] = "Workflow not found.";
                return RedirectToAction(nameof(Workflows));
            }

            var admins = await _adminService.GetAllAdminsAsync();
            var model = new CreateWorkflowViewModel
            {
                Workflow = wf,
                Admins = admins.Select(a => new AdminListItem
                {
                    Id = a.AdminId,
                    Label = a.Email
                }).ToList(),
                SelectedAdminIds = wf.WorkflowAdmins?.Select(wa => wa.AdminId).ToList() ?? new List<int>()
            };

            return View(model);
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkflow(CreateWorkflowViewModel model)
        {
            ModelState.Remove("Workflow.CreatedByAdmin");

            if (!ModelState.IsValid)
            {
                await HydrateAdminsAsync(model);
                return View(model);
            }

            try
            {
                var existing = await _workflowService.GetWorkflowByIdAsync(model.Workflow.WorkflowId);
                if (existing == null)
                {
                    TempData["Error"] = "Workflow not found.";
                    return RedirectToAction(nameof(Workflows));
                }

                existing.Name = model.Workflow.Name;
                existing.Type = model.Workflow.Type;
                existing.UpdatedAt = DateTime.UtcNow;

                await _workflowService.UpdateWorkflowAsync(existing);

                await _workflowService.AssignTasksToWorkflowAsync(
                    existing.WorkflowId,
                    model.SelectedAdminIds?.Distinct().ToList() ?? new List<int>());

                TempData["Success"] = "Workflow updated successfully.";
                return RedirectToAction(nameof(Workflows));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the workflow.";
                await HydrateAdminsAsync(model);
                return View(model);
            }
        }

        // ---------- helpers ----------
        private async Task HydrateAdminsAsync(CreateWorkflowViewModel model)
        {
            var admins = await _adminService.GetAllAdminsAsync();
            model.Admins = admins.Select(a => new AdminListItem
            {
                Id = a.AdminId,
                Label = a.Email
            }).ToList();
        }

        private int GetCurrentAdminId()
        {

            var idFromSession = HttpContext.Session.GetInt32("AdminId");
            if (idFromSession.HasValue) return idFromSession.Value;

            var claimVal = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimVal, out var id)) return id;


            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                var principal = _jwtUtils.ValidateToken(token);
                var claim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(claim, out id)) return id;
            }

            throw new UnauthorizedAccessException("Invalid or missing admin id.");
        }
    }
}
