using WDMS.Domain.Enums;

namespace WDMS.Application.DTOs
{
    public class AdminResponse
    {
        public int AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public AccessLevel AccessLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
