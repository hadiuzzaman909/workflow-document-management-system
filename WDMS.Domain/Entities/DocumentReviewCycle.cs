using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class DocumentReviewCycle
    {
        public int CycleId { get; set; }  // This will be the primary key
        public int DocumentId { get; set; }  // Foreign Key to Document
        public int CycleNo { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAtUtc { get; set; }

        // Navigation Property
        public Document Document { get; set; }  // Navigation to Document
    }
}
