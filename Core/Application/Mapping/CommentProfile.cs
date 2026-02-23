using AutoMapper;
using Application.DTOs.Comment;
using Domain.Entities.Models;

namespace Application.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentDto, Comment>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt,
                opt => opt.Ignore())
            .ForMember(dest => dest.EditedAt,
                opt => opt.Ignore())
            .ForMember(dest => dest.IsEdited,
                opt => opt.Ignore());
    }
}
