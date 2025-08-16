using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Domain.Enums
{
    public enum ApprovalStatus : byte
    {
        Pending = 0, 
        Approved = 1,  
        Rejected = 2,
        Waiting = 3    
    }
}