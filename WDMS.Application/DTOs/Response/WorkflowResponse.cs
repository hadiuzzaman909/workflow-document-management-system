using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Application.DTOs.Response
{
    public class WorkflowResponse
    {
        public int WorkflowId { get; set; }
        public string Name { get; set; } = "";
        public WDMS.Domain.Enums.WorkflowType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByEmail { get; set; } = "";  
    }

}
