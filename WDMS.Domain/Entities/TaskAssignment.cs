using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class TaskAssignment
    {
        public int TaskAssignmentId { get; set; }
        public int WorkflowId { get; set; }
        public int AdminId { get; set; }
        public TaskState Status { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        public Admin Admin { get; set; }
        public Workflow Workflow { get; set; }
    }
}
