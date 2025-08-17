using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class DocumentReviewCycle
    {
        public int CycleId { get; set; }  
        public int DocumentId { get; set; }  
        public int CycleNo { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAtUtc { get; set; }


        public DocumentType Document { get; set; }  
    }
}