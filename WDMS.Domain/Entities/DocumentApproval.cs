using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class DocumentApproval
    {
            public int ApprovalId { get; set; }  
            public int CycleId { get; set; }
            public int AdminId { get; set; }
            public int? SequenceNo { get; set; } 
            public ApprovalStatus Status { get; set; }
            public DateTime? ActionAtUtc { get; set; }
            public string? Comments { get; set; }

            public DocumentReviewCycle DocumentReviewCycle { get; set; } 
            public Admin Admin { get; set; }  
    }
    
}