using System.ComponentModel.DataAnnotations;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public AccessLevel AccessLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public List<WorkflowAdmin> WorkflowAdmins { get; set; } = new List<WorkflowAdmin>();
    }
}