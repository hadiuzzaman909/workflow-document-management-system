
using WDMS.Domain.Entities;

namespace WDMS.Admin.Models
{
    public class AdminListItem
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty; 
    }

    public class CreateWorkflowViewModel
    {
        public WDMS.Domain.Entities.Workflow Workflow { get; set; } = new();
        public List<AdminListItem> Admins { get; set; } = new();
        public List<int> SelectedAdminIds { get; set; } = new();
    }
}
