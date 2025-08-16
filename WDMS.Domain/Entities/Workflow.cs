using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class Workflow
    {
        public int WorkflowId { get; set; }
        public string Name { get; set; } = string.Empty;
        public WorkflowType Type { get; set; }
        public int CreatedByAdminId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Admin CreatedByAdmin { get; set; }
    }

}
