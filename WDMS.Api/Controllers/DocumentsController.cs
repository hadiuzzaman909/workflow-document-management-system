using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;
using WDMS.Application.Services;

namespace WDMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }


        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)] 
        [RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<ActionResult<DocumentResponse>> UploadDocument(
            [FromForm] DocumentCreateRequest documentRequest,
            CancellationToken ct)
        {
            try
            {
                var createdByAdminId = GetCurrentAdminId();
                var created = await _documentService.CreateDocumentAsync(documentRequest, createdByAdminId, ct);

                return CreatedAtAction(nameof(GetDocumentById),
                    new { id = created.DocumentId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while uploading the document.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentResponse>> GetDocumentById(int id, CancellationToken ct)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id, ct);
                if (document == null)
                    return NotFound(new { message = $"Document with ID {id} not found." });

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving the document.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id:int}/file")]
        public async Task<IActionResult> Download(int id, CancellationToken ct)
        {
            try
            {
                var tuple = await _documentService.OpenAsync(id, ct);
                if (tuple == null) return NotFound(new { message = "File not found." });

                var (stream, contentType, fileName) = tuple.Value;
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while downloading the file.",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentResponse>>> GetAllDocuments(CancellationToken ct)
        {
            try
            {
                var documents = await _documentService.GetAllDocumentsAsync(ct);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving documents.",
                    error = ex.Message
                });
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDocument(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _documentService.DeleteDocumentAsync(id, ct);
                if (!ok) return NotFound(new { message = $"Document with ID {id} not found." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while deleting the document.",
                    error = ex.Message
                });
            }
        }

        private int GetCurrentAdminId()
        {

            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;
            throw new UnauthorizedAccessException("Invalid or missing admin id in token.");
        }
    }
}
