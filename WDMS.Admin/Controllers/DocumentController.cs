using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WDMS.Admin.Models;
using WDMS.Application.DTOs.Request;
using WDMS.Application.Services;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Repositories.IRepositories;
using WDMS.Infrastructure.Utils;

namespace WDMS.Admin.Controllers
{
    [RequireLogin]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IWorkflowService _workflowService;
        private readonly IGenericRepository<DocumentType> _documentTypeRepo;
        private readonly JwtUtils _jwtUtils;

        public DocumentController(
            IDocumentService documentService,
            IWorkflowService workflowService,
            IGenericRepository<DocumentType> documentTypeRepo,
            JwtUtils jwtUtils)
        {
            _documentService = documentService;
            _workflowService = workflowService;
            _documentTypeRepo = documentTypeRepo;
            _jwtUtils = jwtUtils;
        }

  
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var docs = await _documentService.GetAllDocumentsAsync();
            return View(docs); 
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildCreateViewModelAsync(new DocumentCreateViewModel());
            return View(vm);
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await BuildCreateViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                var currentAdminId = GetCurrentAdminId(); 
                var req = new DocumentCreateRequest
                {
                    DocumentTypeId = vm.DocumentTypeId,
                    WorkflowId = vm.WorkflowId,
                    Name = vm.Name,
                    File = vm.File
                };

                await _documentService.CreateDocumentAsync(req, currentAdminId);
                TempData["Success"] = "Document uploaded successfully.";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to upload document: {ex.Message}");
                await BuildCreateViewModelAsync(vm);
                return View(vm);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var tuple = await _documentService.OpenAsync(id);
            if (tuple == null)
            {
                TempData["Error"] = "File not found.";
                return RedirectToAction(nameof(Index));
            }

            var (stream, contentType, fileName) = tuple.Value;
            return File(stream, contentType, fileName);
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _documentService.DeleteDocumentAsync(id);
                if (!ok)
                {
                    TempData["Error"] = "Document not found.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Document deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete document: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<DocumentCreateViewModel> BuildCreateViewModelAsync(DocumentCreateViewModel vm)
        {
            var types = await _documentTypeRepo.GetAllAsync();
            var wfs = await _workflowService.GetAllWorkflowsAsync();

            vm.DocumentTypes = types.Select(t => new SelectListItem
            {
                Value = t.DocumentTypeId.ToString(),
                Text = t.Name
            }).ToList();

            vm.Workflows = wfs.Select(w => new SelectListItem
            {
                Value = w.WorkflowId.ToString(),
                Text = w.Name
            }).ToList();

            return vm;
        }

        private int GetCurrentAdminId()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                var principal = _jwtUtils.ValidateToken(token);
                var idStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idStr, out var idFromSession)) return idFromSession;
            }

            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;

            throw new UnauthorizedAccessException("Invalid or missing admin id.");
        }
    }
}
