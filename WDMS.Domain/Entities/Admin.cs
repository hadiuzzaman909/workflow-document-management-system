using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Domain.Entities
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public AccessLevel AccessLevel { get; set; }
        public bool IsActive { get; set; }
    }
}
