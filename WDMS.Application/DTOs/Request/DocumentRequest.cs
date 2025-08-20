using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WDMS.Application.DTOs.Request
{
    public class DocumentCreateRequest
    {
        [Required] public int DocumentTypeId { get; set; }
        [Required] public int WorkflowId { get; set; }

        [MaxLength(255)] public string? Name { get; set; }

        [Required] public IFormFile File { get; set; } = default!;
    }
}
