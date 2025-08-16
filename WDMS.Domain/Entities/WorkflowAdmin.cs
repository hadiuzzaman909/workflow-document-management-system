using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Domain.Entities
{
    public class WorkflowAdmin
    {
        public int WorkflowId { get; set; }
        public int AdminId { get; set; }
        public int? SequenceNo { get; set; } 
        public bool IsActive { get; set; } = true;

        public Workflow Workflow { get; set; }  
        public Admin Admin { get; set; }
    }
}
