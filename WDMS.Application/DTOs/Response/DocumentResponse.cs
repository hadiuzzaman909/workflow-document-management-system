using WDMS.Domain.Enums;

namespace WDMS.Application.DTOs.Response
{
    public class DocumentResponse
    {
        public int DocumentId { get; set; }
        public Guid DocumentUid { get; set; }
        public string Name { get; set; } = string.Empty;

        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;

        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }

        public int CreatedByAdminId { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public DocumentState State { get; set; }
    }
}