using Microsoft.AspNetCore.Mvc;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Entities;

namespace WDMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentType>>> GetDocumentTypes()
        {
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();
            return Ok(documentTypes);
        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentType([FromBody] DocumentType documentType)
        {
            if (documentType == null)
            {
                return BadRequest(new { message = "Document type request body is null." });
            }

            await _documentTypeService.CreateDocumentTypeAsync(documentType);
            return CreatedAtAction(nameof(GetDocumentTypes), new { id = documentType.DocumentTypeId }, documentType);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDocumentType(int id, [FromBody] DocumentType documentType)
        {
            if (id != documentType.DocumentTypeId)
            {
                return BadRequest(new { message = "Document type ID mismatch." });
            }

            await _documentTypeService.UpdateDocumentTypeAsync(documentType);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocumentType(int id)
        {
            await _documentTypeService.DeleteDocumentTypeAsync(id);
            return NoContent();
        }
    }
}
