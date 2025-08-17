using System.ComponentModel.DataAnnotations;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public Guid DocumentUid { get; set; } = Guid.NewGuid();

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; } = null!;

        [Required]
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; } = null!;

        // File info
        [Required, MaxLength(1024)]
        public string FilePath { get; set; } = string.Empty;         
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;
        [MaxLength(127)]
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }

        // Audit
        [Required]
        public int CreatedByAdminId { get; set; }
        public Admin CreatedByAdmin { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;


        public DocumentState State { get; set; } = DocumentState.Pending;
    }
}
