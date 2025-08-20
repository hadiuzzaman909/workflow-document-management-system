using AutoMapper;
using WDMS.Application.DTOs.Response;
using WDMS.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Document, DocumentResponse>()
            .ForMember(d => d.DocumentTypeName, o => o.MapFrom(s => s.DocumentType.Name))
            .ForMember(d => d.WorkflowName, o => o.MapFrom(s => s.Workflow.Name))
            .ForMember(d => d.CreatedByEmail, o => o.MapFrom(s => s.CreatedByAdmin.Email))
            .ForMember(d => d.FileUrl, o => o.MapFrom(s => "/" + s.FilePath.TrimStart('/')));


        // MappingProfile.cs
        CreateMap<Workflow, WorkflowResponse>()
        .ForMember(d => d.CreatedByEmail, o => o.MapFrom(s => s.CreatedByAdmin.Email));

    }
}
