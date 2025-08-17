using System.ComponentModel.DataAnnotations;

namespace WDMS.Domain.Entities
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<Document> Documents { get; set; } = new();

    }
}
