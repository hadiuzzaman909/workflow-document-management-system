namespace WDMS.Domain.Entities
{
    public class WorkflowAdmin
    {
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}