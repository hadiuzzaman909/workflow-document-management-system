using System;

namespace WDMS.Application.DTOs.Request
{
    public class DocumentRequest
    {
        public int DocumentTypeId { get; set; } 
        public int WorkflowId { get; set; }        
        public string? Name { get; set; }          
        public string? FilePath { get; set; }      
    }
}