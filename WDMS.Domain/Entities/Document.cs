using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
        public class Document
        {
            public int DocumentId { get; set; }
            public Guid DocumentUid { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = string.Empty;
            public int DocumentTypeId { get; set; }
            public string FilePath { get; set; } = string.Empty;
            public int WorkflowId { get; set; }
            public DocumentStatus Status { get; set; }
            public int RevisionNo { get; set; } = 1;
            public int CreatedByAdminId { get; set; }
            public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAtUtc { get; set; }

            public Admin CreatedByAdmin { get; set; } 
            public Workflow Workflow { get; set; }  

    }
}
