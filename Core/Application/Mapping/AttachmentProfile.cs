using Application.DTOs.Attachment;
using Domain.Entities.Models;
using AutoMapper;
using Domain.Entities.Enums;

namespace Application.Mapping;

public class AttachmentProfile : Profile
{
    public AttachmentProfile()
    {
        CreateMap<Attachment,AttachmentDto>();
        CreateMap<CreateAttachmentDto, Attachment>()
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => Enum.Parse<AttachmentType>(src.Type)))
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,
                opt => opt.Ignore());
    }
}
