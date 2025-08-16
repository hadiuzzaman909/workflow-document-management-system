using System.ComponentModel.DataAnnotations;
using WDMS.Domain.Enums;

namespace WDMS.Application.DTOs
{

    public class AdminRequest
    {
        public int AdminId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public AccessLevel AccessLevel { get; set; }
    }
   
}
