using System.ComponentModel.DataAnnotations;
using WDMS.Domain.Entities;
using WDMS.Domain.Enums;

public class Workflow
{
    [Key]
    public int WorkflowId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public WorkflowType Type { get; set; }

    public int CreatedByAdminId { get; set; } 

    public Admin CreatedByAdmin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    public List<WorkflowAdmin> WorkflowAdmins { get; set; } = new List<WorkflowAdmin>();
}
