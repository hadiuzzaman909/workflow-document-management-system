using Microsoft.AspNetCore.Mvc;
using WDMS.Application.DTOs.Request;
using WDMS.Application.DTOs.Response;
using WDMS.Application.Services.IServices;
using WDMS.Infrastructure.Services;

namespace WDMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorage _fileStorage;

        public DocumentController(IDocumentService documentService, IFileStorage fileStorage)
        {
            _documentService = documentService;
            _fileStorage = fileStorage;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<DocumentResponse>> UploadDocument([FromForm] DocumentRequest documentRequest, IFormFile file)
        {
            try
            {
                var filePath = await _fileStorage.SaveAsync(file, "documents");

                documentRequest.FilePath = filePath;

                var document = await _documentService.CreateDocumentAsync(documentRequest);

                return new ActionResult<DocumentResponse>(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while uploading the document.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentResponse>> GetDocumentById(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);

                if (document == null)
                {
                    return NotFound(new { message = $"Document with ID {id} not found." });
                }

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the document.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentResponse>>> GetAllDocuments()
        {
            try
            {
                var documents = await _documentService.GetAllDocumentsAsync();

                if (documents == null || documents.Count == 0)
                {
                    return NotFound(new { message = "No documents found." });
                }

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving documents.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                await _documentService.DeleteDocumentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the document.", error = ex.Message });
            }
        }
    }
}
