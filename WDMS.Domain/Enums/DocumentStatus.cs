using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Domain.Enums
{
    public enum DocumentStatus : byte
    {
        Draft = 0,     
        InReview = 1,  
        Approved = 2, 
        Rejected = 3 
    }
}