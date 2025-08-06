using AutoMapper;
using PostService.Data.Models;
using PostService.DTOs;

namespace PostService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<PostDto, Post>();
            CreateMap<Post, PostDto>();
            CreateMap<AddPostDto, Post>();
            CreateMap<Post, AddPostDto>();
        }
    }
}
