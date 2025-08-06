using AutoMapper;
using CommentService.Data.Model;
using CommentService.DTOs;

namespace CommentService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
            CreateMap<Comment, AddCommentDto>();
            CreateMap<AddCommentDto, Comment>();
            CreateMap<CommentFullInfo, CommentFullInfoDto>();
            CreateMap<CommentFullInfoDto, CommentFullInfo>();
        }
    }
}
