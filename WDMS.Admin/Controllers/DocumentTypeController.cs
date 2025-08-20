using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;

namespace WDMS.Admin.Controllers
{
    public class DocumentTypeController : Controller
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        [RequireLogin]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();
            return View(documentTypes);
        }

        [RequireLogin]
        [HttpGet]
        public IActionResult CreateDocumentType()
        {
            return View(new DocumentType());
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocumentType(DocumentType documentType)
        {
            if (!ModelState.IsValid) return View(documentType);

            try
            {
                await _documentTypeService.CreateDocumentTypeAsync(documentType);
                TempData["Success"] = "Document type created successfully.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the document type.";
                return RedirectToAction(nameof(List));
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpGet]
        public async Task<IActionResult> EditDocumentType(int id)
        {
            try
            {
                var documentType = await _documentTypeService.GetDocumentTypeByIdAsync(id);
                if (documentType == null)
                {
                    TempData["Error"] = "Document type not found.";
                    return RedirectToAction(nameof(List));
                }
                return View(documentType);
            }
            catch
            {
                TempData["Error"] = "Unable to load the document type.";
                return RedirectToAction(nameof(List));
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocumentType(DocumentType documentType)
        {
            if (!ModelState.IsValid) return View(documentType);

            try
            {
                await _documentTypeService.UpdateDocumentTypeAsync(documentType);
                TempData["Success"] = "Document type updated successfully.";
                return RedirectToAction(nameof(List));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Document type not found.";
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the document type.";
                return RedirectToAction(nameof(List));
            }
        }

        [RequireLogin]
        [Authorize(Policy = "ReadWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            try
            {
                await _documentTypeService.DeleteDocumentTypeAsync(id);
                TempData["Success"] = "Document type deleted.";
                return RedirectToAction(nameof(List));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Document type not found.";
                return RedirectToAction(nameof(List));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(List));
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the document type.";
                return RedirectToAction(nameof(List));
            }
        }
    }
}
