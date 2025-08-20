using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WDMS.Application.Services.IServices;
using WDMS.Infrastructure.Repositories;
using WDMS.Infrastructure.Utils;

[RequireLogin]
public class DashboardController : Controller
{
    private readonly ITaskAssignmentRepository _taskRepo;
    private readonly IWorkflowService _workflowService;
    private readonly JwtUtils _jwtUtils;

    public DashboardController(ITaskAssignmentRepository taskRepo,
                               IWorkflowService workflowService,
                               JwtUtils jwtUtils)
    {
        _taskRepo = taskRepo;
        _workflowService = workflowService;
        _jwtUtils = jwtUtils;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int adminId = GetCurrentAdminId();
        var tasks = await _taskRepo.GetActivePendingByAdminAsync(adminId);
        return View(tasks); 
    }

    [Authorize(Policy = "ReadWrite")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _workflowService.ApproveTaskAsync(id);
        TempData["Success"] = "Task approved.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ReadWrite")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        await _workflowService.RejectTaskAsync(id);
        TempData["Success"] = "Task rejected.";
        return RedirectToAction(nameof(Index));
    }

    private int GetCurrentAdminId()
    {
        var fromSession = HttpContext.Session.GetInt32("AdminId");
        if (fromSession.HasValue) return fromSession.Value;

        var claimVal = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(claimVal, out var id)) return id;

        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            var principal = _jwtUtils.ValidateToken(token);
            var idStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idStr, out id)) return id;
        }
        throw new UnauthorizedAccessException("Invalid or missing admin id.");
    }
}
