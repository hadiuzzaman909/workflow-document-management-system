using System.ComponentModel.DataAnnotations;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class TaskAssignment
    {
        [Key]
        public int TaskAssignmentId { get; set; }

        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; } = null!;

        public int AdminId { get; set; }
        public Admin Admin { get; set; } = null!;

        public TaskState Status { get; set; } = TaskState.Pending;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

  
        public int StepOrder { get; set; } = 0;    


        public bool IsActive { get; set; } = true;  
    }
}
