using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WDMS.Admin.Models
{
    public class DocumentCreateViewModel
    {
        [Required]
        [Display(Name = "Document Type")]
        public int DocumentTypeId { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public int WorkflowId { get; set; }

        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; } = default!;

        public IEnumerable<SelectListItem> DocumentTypes { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Workflows { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
